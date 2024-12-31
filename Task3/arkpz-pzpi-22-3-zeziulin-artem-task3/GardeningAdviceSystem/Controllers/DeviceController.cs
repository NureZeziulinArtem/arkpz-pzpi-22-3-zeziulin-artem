using Core.Enums;
using Core.Models;
using DAL;
using GardeningAdviceSystem.Models;
using GardeningAdviceSystem.Models.Device;
using GardeningAdviceSystem.Models.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GardeningAdviceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Repository<Device> _deviceRepository;
        private readonly Repository<DeviceLog> _deviceLogRepository;
        private readonly UserManager<Account> _userManager;
        private readonly Repository<Plant> _plantRepository;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(UnitOfWork unitOfWork, UserManager<Account> userManager, 
            ILogger<DeviceController> logger)
        {
            _unitOfWork = unitOfWork;
            _deviceRepository = unitOfWork.DeviceRepository;
            _deviceLogRepository = unitOfWork.DeviceLogRepository;
            _plantRepository = unitOfWork.PlantRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateDevise([FromBody] CreateDeviceModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to create device - model not received");
                return BadRequest();
            }

            var device = new Device()
            {
                Name = model.Name,
                Description = model.Description
            };

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogInformation("Failed to create device - user not found");
                    return Unauthorized();
                }

                device.AccountId = user.Id;

                await _deviceRepository.AddAsync(device);
                await _unitOfWork.Save();

                // TODO: connect to real device

                return Ok(device.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> EditDevice(EditDeviceModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to edit device - model not received");
                return BadRequest();
            }

            try
            {
                var device = await _deviceRepository.GetByIdAsync(model.Id);

                if (device == null)
                {
                    _logger.LogInformation("Failed to edit device - device not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null || device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to edit device - wrong user");
                    return Unauthorized();
                }

                device.Name = model.Name;
                device.Description = model.Description;
                device.PlantId = model.PlantId;

                await _deviceRepository.UpdateAsync(device);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to delete device - id not received");
                return BadRequest();
            }

            try
            {
                var device = await _deviceRepository.GetByIdAsync(id);

                if (device == null)
                {
                    _logger.LogInformation("Failed to delete device - device not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null || device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to delete device - wrong user");
                    return Unauthorized();
                }

                await _deviceRepository.DeleteAsync(device);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        // retrieves all devices, created by current user
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve devices - user not found");
                    return Unauthorized();
                }

                var models = user.Devices
                    .Select(d => new ListItemModel()
                    {
                        Id = d.Id,
                        Name = d.Name
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDevice(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to retrieve device - id not received");
                return BadRequest();
            }

            try
            {
                var device = await _deviceRepository.GetByIdAsync(id);

                if (device == null)
                {
                    _logger.LogInformation("Failed to retrieve device - device not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null || device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to retrieve device - wrong user");
                    return Unauthorized();
                }

                var model = new DeviceModel()
                {
                    Name = device.Name,
                    Description = device.Description,
                    PlantId = device.PlantId,
                    PlantName = device.Plant?.Name
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("log-data")]
        public async Task<IActionResult> LogData([FromBody] CreateDeviceLogModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to log device data - model not received");
                return BadRequest();
            }

            var log = new DeviceLog()
            {
                DeviceId = model.DeviceId,
                Ph = model.Ph,
                Moisture = model.Moisture,
                Recorded = DateTime.Now
            };

            try
            {
                await _deviceLogRepository.AddAsync(log);
                await _unitOfWork.Save();

                return Ok(log.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("logs/{deviceId}")]
        public async Task<IActionResult> GetDeviceLogs(int deviceId)
        {
            if (deviceId == 0)
            {
                _logger.LogError("Failed to retrieve device logs - deviceId not received");
                return BadRequest();
            }

            try
            {
                var logs = await _deviceLogRepository.GetAsync(
                    filter: l => l.DeviceId == deviceId,
                    orderBy: l => l.OrderByDescending(l => l.Recorded));

                if (logs == null)
                {
                    _logger.LogInformation("Failed to retrieve device logs - device logs not found");
                    return NotFound();
                }

                var models = logs.Select(l => new DeviceLogModel()
                {
                    Recorded = l.Recorded,
                    Ph = l.Ph,
                    Moisture = l.Moisture
                });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("predict")]
        [Authorize]
        // takes the type of prediction (which value is predicted),
        // device id, the amount of days to concider for prediction and
        // the amount of days into the future to predict for and
        // returns the list of values
        public async Task<IActionResult> PredictValues(PredictionType type, 
            int deviceId, int considerDays, int forDays)
        {
            if (deviceId == 0)
            {
                _logger.LogError("Failed to make a prediction - deviceId not received");
                return BadRequest();
            }

            if (considerDays == 0)
            {
                _logger.LogError("Failed to make a prediction - considerDays not received");
                return BadRequest();
            }

            if (forDays == 0)
            {
                _logger.LogError("Failed to make a prediction - forDays not received");
                return BadRequest();
            }

            try
            {
                var logs = await _deviceLogRepository.GetAsync(
                    filter: l => l.DeviceId == deviceId,
                    orderBy: l => l.OrderBy(l => l.Recorded));

                if (logs == null)
                {
                    _logger.LogInformation("Failed to retrieve device logs - device logs not found");
                    return NotFound();
                }

                List<double> values;
                if (type == PredictionType.Moisture)
                {
                    values = logs.TakeLast(considerDays).Select(l => (double)l.Moisture).ToList();
                }
                else
                {
                    values = logs.TakeLast(considerDays).Select(l => (double)l.Ph).ToList();
                }

                var models = LinearRegressionPrediction(values, forDays);

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("advice-plants")]
        [Authorize]
        // takes the device id and retrieves it's latest log
        // to determine, which plants can pe planted in this soil.
        // returns the list of plants' ids
        public async Task<IActionResult> AdvicePlants(int deviceId)
        {
            if (deviceId == 0)
            {
                _logger.LogError("Failed to form an advice - deviceId not received");
                return BadRequest();
            }

            try
            {
                var device = await _deviceRepository.GetByIdAsync(deviceId);
                if (device == null)
                {
                    _logger.LogInformation("Failed to form an advice - device not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null || device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to form an advice - wrong user");
                    return Unauthorized();
                }

                var lastLog = device.Logs.MaxBy(d => d.Recorded);
                if (lastLog == null)
                {
                    _logger.LogInformation("Failed to form an advice - no logs found");
                    return NotFound();
                }

                var plants = await _plantRepository.GetAsync(p => 
                    (p.AccountId == null || p.AccountId == user.Id)
                    && p.MaxMoisture >= lastLog.Moisture
                    && p.MinMoisture <= lastLog.Moisture
                    && p.MaxPh >= lastLog.Ph
                    && p.MinPh <= lastLog.Ph);

                if (plants == null)
                {
                    _logger.LogInformation("Failed to retrieve plants - plants not found");
                    return NotFound();
                }

                return Ok(plants.Select(p => p.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("advice-for-plant")]
        [Authorize]
        // takes the device id and the plant id
        // retrieves the device's latest log
        // to determine the difference between the plant's
        // requirements and the current state of soil
        public async Task<IActionResult> AdviceForPlant(int deviceId, int plantId)
        {
            if (deviceId == 0)
            {
                _logger.LogError("Failed to form an advice - deviceId not received");
                return BadRequest();
            }

            if (plantId == 0)
            {
                _logger.LogError("Failed to form an advice - plantId not received");
                return BadRequest();
            }

            try
            {
                var device = await _deviceRepository.GetByIdAsync(deviceId);
                if (device == null)
                {
                    _logger.LogInformation("Failed to form an advice - device not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null || device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to form an advice - wrong user");
                    return Unauthorized();
                }

                var lastLog = device.Logs.MaxBy(d => d.Recorded);
                if (lastLog == null)
                {
                    _logger.LogInformation("Failed to form an advice - no logs found");
                    return NotFound();
                }

                var plant = await _plantRepository.GetByIdAsync(plantId);

                if (plant == null)
                {
                    _logger.LogInformation("Failed to retrieve plants - plants not found");
                    return NotFound();
                }

                var model = new PlantAdviceModel()
                {
                    Moisture = lastLog.Moisture,
                    Ph = lastLog.Ph,
                    MinMoisture = plant.MinMoisture,
                    MaxMoisture = plant.MaxMoisture,
                    MinPh = plant.MinPh,
                    MaxPh = plant.MaxPh
                };

                var averagePh = (model.MinPh + model.MaxPh) / 2;
                model.IdealPhChange = averagePh - model.Ph;

                var averageMoisture = (model.MinMoisture + model.MaxMoisture) / 2;
                model.IdealMoistureChange = averageMoisture - model.Moisture;

                if (model.Moisture > model.MaxMoisture)
                {
                    model.MinMoistureChange = model.MaxMoisture - model.Moisture;
                }
                else if (model.Moisture < model.MinMoisture)
                {
                    model.MinMoistureChange = model.Moisture - model.MinMoisture;
                }

                if (model.Ph > model.MaxPh)
                {
                    model.MinPhChange = model.MaxPh - model.Ph;
                }
                else if (model.Ph < model.MinPh)
                {
                    model.MinPhChange = model.Ph - model.MinPh;
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private List<double> LinearRegressionPrediction(List<double> values, int daysToPredict)
        {
            int n = values.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;

            for (int i = 0; i < n; i++)
            {
                sumX += i;
                sumY += values[i];
                sumXY += i * values[i];
                sumXX += i * i;
            }

            double slope = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;

            var predictions = new List<double>();
            for (int i = 0; i < daysToPredict; i++)
            {
                double futureX = n + i; // Future days
                predictions.Add(slope * futureX + intercept);
            }

            return predictions;
        }
    }
}
