using Core;
using Core.Models;
using DAL;
using GardeningAdviceSystem.Models.Account;
using Microsoft.AspNetCore.Authorization;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<Account> userManager,
            RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger, 
            SignInManager<Account> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to register admin - model not received");
                return BadRequest();
            }

            if (model.Password.Trim() != model.ConfirmPassword.Trim())
            {
                return BadRequest();
            }

            var account = new Account()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = (Guid.NewGuid()).ToString()
            };

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    _logger.LogInformation("Failed to register - user {email} already exists", model.Email);
                    return BadRequest();
                }

                var userResult = await _userManager.CreateAsync(account, model.Password);

                if (!userResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, userResult.Errors);
                }

                var setupRolesResult = await SetupUserRoles();

                if (!setupRolesResult)
                {
                    await _userManager.DeleteAsync(account);

                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to setup roles");
                }

                var roleResult = await _userManager.AddToRoleAsync(account, 
                    model.AdminType == AdminType.Admin ? UserRoles.Admin : UserRoles.DbAdmin);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(account);

                    return StatusCode(StatusCodes.Status500InternalServerError, roleResult.Errors);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("register-first-admin")]
        public async Task<IActionResult> RegisterFirstAdmin([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to register admin - model not received");
                return BadRequest();
            }

            if (model.Password.Trim() != model.ConfirmPassword.Trim())
            {
                return BadRequest();
            }

            try
            {
                var admins = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);

                if (admins != null && admins.Count > 0)
                {
                    return BadRequest("First admin already added");
                }

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    _logger.LogInformation("Failed to register - user {email} already exists", model.Email);
                    return BadRequest();
                }

                var account = new Account()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = (Guid.NewGuid()).ToString()
                };

                var userResult = await _userManager.CreateAsync(account, model.Password);

                if (!userResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, userResult.Errors);
                }

                var setupRolesResult = await SetupUserRoles();

                if (!setupRolesResult)
                {
                    await _userManager.DeleteAsync(account);

                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to setup roles");
                }

                var roleResult = await _userManager
                                .AddToRoleAsync(account, UserRoles.Admin);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(account);

                    return StatusCode(StatusCodes.Status500InternalServerError, roleResult.Errors);
                }

                await _signInManager.SignInAsync(account, isPersistent: false);

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
        public async Task<IActionResult> GetAccount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

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

            if (model.Password.Trim() != model.ConfirmPassword.Trim())
            {
                return BadRequest();
            }

            var account = new Account()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = (Guid.NewGuid()).ToString()
            };

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    _logger.LogInformation("Failed to register - user {email} already exists", model.Email);
                    return BadRequest();
                }

                var userResult = await _userManager.CreateAsync(account, model.Password);

                if (!userResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, userResult.Errors);
                }

                var setupRolesResult = await SetupUserRoles();

                if (!setupRolesResult)
                {
                    await _userManager.DeleteAsync(account);

                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to setup roles");
                }

                var roleResult = await _userManager
                                .AddToRoleAsync(account, UserRoles.User);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(account);

                    return StatusCode(StatusCodes.Status500InternalServerError, roleResult.Errors);
                }

                await _signInManager.SignInAsync(account, isPersistent: false);

                return Ok();
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

                var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.Password);

                if (!passwordCheckResult)
                {
                    _logger.LogInformation("Failed to login - wrong password");

                    return BadRequest("Wrong password");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] EditAccountModel model)
        {
            if (model == null)
            {
                _logger.LogError("Model was not received");
                return BadRequest();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

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
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null)
            {
                _logger.LogError("Model was not received");
                return BadRequest();
            }

            if (model.NewPassword.Trim() != model.ConfirmNewPassword.Trim())
            {
                return BadRequest();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogInformation("Failed to change password - user not found");
                    return Unauthorized();
                }

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

        private async Task<bool> SetupUserRoles()
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                }

                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                }

                if (!await _roleManager.RoleExistsAsync(UserRoles.DbAdmin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.DbAdmin));
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
