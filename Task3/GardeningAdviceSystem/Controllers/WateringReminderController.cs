using Core.Models;
using DAL;
using GardeningAdviceSystem.Models;
using GardeningAdviceSystem.Models.WateringReminder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GardeningAdviceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WateringReminderController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Repository<WateringReminder> _wateringReminderRepository;
        private readonly UserManager<Account> _userManager;
        private readonly ILogger<WateringReminderController> _logger;

        public WateringReminderController(UnitOfWork unitOfWork, 
            UserManager<Account> userManager, ILogger<WateringReminderController> logger)
        {
            _unitOfWork = unitOfWork;
            _wateringReminderRepository = unitOfWork.WateringReminderRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateReminder(CreateReminderModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to create reminder - model not received");
                return BadRequest();
            }

            var reminder = new WateringReminder()
            {
                DeviceId = model.DeviceId,
                Regular = model.Regular,
                ReminderDate = model.ReminderDate,
                DayGap = model.DayGap
            };

            try
            {
                await _wateringReminderRepository.AddAsync(reminder);
                await _unitOfWork.Save();

                return Ok(reminder.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> EditReminder(EditReminderModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to edit reminder - model not received");
                return BadRequest();
            }

            try
            {
                var reminder = await _wateringReminderRepository.GetByIdAsync(model.Id);

                if (reminder == null)
                {
                    _logger.LogInformation("Failed to edit reminder - reminder not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null || reminder.Device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to edit reminder - wrong user");
                    return Unauthorized();
                }

                reminder.Regular = model.Regular;
                reminder.ReminderDate = model.ReminderDate;
                reminder.DayGap = model.DayGap;

                await _wateringReminderRepository.UpdateAsync(reminder);
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
        public async Task<IActionResult> DeleteReminder(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to delete reminder - id not received");
                return BadRequest();
            }

            try
            {
                var reminder = await _wateringReminderRepository.GetByIdAsync(id);

                if (reminder == null)
                {
                    _logger.LogInformation("Failed to delete reminder - reminder not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null || reminder.Device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to edit reminder - wrong user");
                    return Unauthorized();
                }

                await _wateringReminderRepository.DeleteAsync(reminder);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReminder(int id)
        {
            try
            {
                var reminder = await _wateringReminderRepository.GetByIdAsync(id);

                if (reminder == null)
                {
                    _logger.LogInformation("Failed to retrieve reminder - reminder not found");
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null || reminder.Device.AccountId != user.Id)
                {
                    _logger.LogInformation("Failed to edit reminder - wrong user");
                    return Unauthorized();
                }

                var model = new ReminderModel()
                {
                    DeviceId = reminder.DeviceId,
                    DeviceName = reminder.Device.Name,
                    DayGap = reminder.DayGap,
                    Id = reminder.Id,
                    Regular = reminder.Regular,
                    ReminderDate = reminder.ReminderDate
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReminders()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve reminders - user not found");
                    return Unauthorized();
                }

                var models = user.Devices.SelectMany(d => d.Reminders)
                    .Select(r => new ReminderModel()
                    {
                        DeviceId = r.DeviceId,
                        DeviceName = r.Device.Name,
                        DayGap = r.DayGap,
                        Id = r.Id,
                        Regular = r.Regular,
                        ReminderDate = r.ReminderDate
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("device/{deviceId}")]
        [Authorize]
        public async Task<IActionResult> GetReminders(int deviceId)
        {
            if (deviceId == 0)
            {
                _logger.LogError("Failed to retrieve reminders - deviceId not received");
                return BadRequest();
            }

            try
            {
                var reminders = await _wateringReminderRepository.GetAsync(r => r.DeviceId == deviceId);

                if (reminders == null)
                {
                    _logger.LogInformation("Failed to retrieve reminders - reminders not found");
                    return NotFound();
                }

                var models = reminders.Select(r => new ReminderModel()
                    {
                        DeviceId = r.DeviceId,
                        DeviceName = r.Device.Name,
                        DayGap = r.DayGap,
                        Id = r.Id,
                        Regular = r.Regular,
                        ReminderDate = r.ReminderDate
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
