using Core;
using Core.Models;
using DAL;
using GardeningAdviceSystem.Models.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GardeningAdviceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<Account> userManager, 
            RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // TODO: configure roles with RoleManager

        [HttpGet]
        // must be authorized
        public async Task<IActionResult> GetAccount()
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve account - user not found");
                    return Unauthorized();
                }

                var model = new AccountModel()
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to register user - model not received");
                return BadRequest();
            }

            // check passwords

            var account = new Account()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = new Guid().ToString()
            };

            try
            {
                var result = await _userManager.CreateAsync(account, model.Password);
                // add user to role
                // start user session

                return result.Succeeded? Ok() : StatusCode(StatusCodes
                    .Status500InternalServerError, result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to login - model not received");
                return BadRequest();
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogInformation("Failed to login - user {email} not found", model.Email);
                    return NotFound();
                }

                // verify passwords
                // start user session

                return Ok(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("logout")]
        // must be authorized
        public async Task<IActionResult> Logout()
        {
            // end session

            return Ok();
        }

        [HttpPut("edit")]
        // must be authorized
        public async Task<IActionResult> Edit([FromBody] EditAccountModel model)
        {
            if (model == null)
            {
                _logger.LogError("Model was not received");
                return BadRequest();
            }

            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to edit account - user not found");
                    return Unauthorized();
                }

                user.Name = model.Name;
                user.Surname = model.Surname;

                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded ? Ok() : StatusCode(StatusCodes
                    .Status500InternalServerError, result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("change-password")]
        // must be authorized
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null)
            {
                _logger.LogError("Model was not received");
                return BadRequest();
            }

            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to change password - user not found");
                    return Unauthorized();
                }

                // check new passwords

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                return result.Succeeded ? Ok() : StatusCode(StatusCodes
                    .Status500InternalServerError, result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
