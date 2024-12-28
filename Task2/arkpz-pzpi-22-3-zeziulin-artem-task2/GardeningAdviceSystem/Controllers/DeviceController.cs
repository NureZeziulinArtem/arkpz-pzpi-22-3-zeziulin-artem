using Core.Enums;
using Core.Models;
using DAL;
using GardeningAdviceSystem.Models;
using GardeningAdviceSystem.Models.Device;
using GardeningAdviceSystem.Models.Plant;
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
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(UnitOfWork unitOfWork, UserManager<Account> userManager, 
            ILogger<DeviceController> logger)
        {
            _unitOfWork = unitOfWork;
            _deviceRepository = unitOfWork.DeviceRepository;
            _deviceLogRepository = unitOfWork.DeviceLogRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        // must be authorized
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
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

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
        // must be authorized
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
        // must be authorized
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
        // must be authorized
        // retrieves all devices, created by current user
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

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
        // must be authorized
        // retrieves all devices, created by current user
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

                // check that the plant belongs to current user

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
                var logs = await _deviceLogRepository.GetAsync(l => l.DeviceId == deviceId);

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
        // must be authorized
        // takes the type of prediction (which value is predicted),
        // device id, the amount of days to concider for prediction and
        // the amount of days into the future to predict for and
        // returns the list of values
        public async Task<IActionResult> PredictValues(PredictionType type, 
            int deviceId, int conciderDays, int forDays)
        {
            return Ok(0);
        }

        [HttpGet("advice-plants")]
        // must be authorized
        // takes the device id and retrieves it's latest log
        // to determine, which plants can pe planted in this soil.
        // returns the list of plants' ids
        public async Task<IActionResult> AdvicePlants(int deviceId)
        {
            return Ok(0);
        }

        [HttpGet("advice-for-plant")]
        // must be authorized
        // takes the device id and the plant id
        // retrieves the device's latest log
        // to determine the difference between the plant's
        // requirements and the current state of soil
        public async Task<IActionResult> AdviceForPlant(int deviceId, int plantId)
        {
            return Ok(0);
        }
    }
}
