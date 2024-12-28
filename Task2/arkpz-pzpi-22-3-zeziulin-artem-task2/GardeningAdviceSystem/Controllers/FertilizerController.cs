using Core.Models;
using DAL;
using GardeningAdviceSystem.Models;
using GardeningAdviceSystem.Models.Fertilizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GardeningAdviceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FertilizerController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Repository<Fertilizer> _fertilizerRepository;
        private readonly Repository<CartItem> _cartItemRepository;
        private readonly FileRepository _fileRepository;
        private readonly UserManager<Account> _userManager;
        private readonly ILogger<FertilizerController> _logger;

        public FertilizerController(UnitOfWork unitOfWork, FileRepository fileRepository,
            UserManager<Account> userManager, ILogger<FertilizerController> logger)
        {
            _unitOfWork = unitOfWork;
            _fertilizerRepository = unitOfWork.FertilizerRepository;
            _cartItemRepository = unitOfWork.CartItemRepository;
            _fileRepository = fileRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        // must be authorized as an admin
        public async Task<IActionResult> CreateFertilizer(CreateFertilizerModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to create fertilizer - model not received");
                return BadRequest();
            }

            var fertilizer = new Fertilizer()
            {
                Name = model.Name,
                Description = model.Description,
                Ph = model.Ph,
                Size = model.Size,
                Price = model.Price,
            };

            try
            {
                await _fertilizerRepository.AddAsync(fertilizer);
                await _unitOfWork.Save();

                return Ok(fertilizer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit")]
        // must be authorized as an admin
        public async Task<IActionResult> EditFertilizer(EditFertilizerModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to edit fertilizer - model not received");
                return BadRequest();
            }

            try
            {
                var fertilizer = await _fertilizerRepository.GetByIdAsync(model.Id);

                if (fertilizer == null)
                {
                    _logger.LogInformation("Failed to edit fertilizer - fertilizer not found");
                    return NotFound();
                }

                fertilizer.Name = model.Name;
                fertilizer.Description = model.Description;
                fertilizer.Ph = model.Ph;
                fertilizer.Price = model.Price;
                fertilizer.Size = model.Size;

                await _fertilizerRepository.UpdateAsync(fertilizer);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit-image")]
        // must be authorized as an admin
        public async Task<IActionResult> EditFertilizerImage(EditImageModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to edit fertilizer image - model not received");
                return BadRequest();
            }

            try
            {
                var fertilizer = await _fertilizerRepository.GetByIdAsync(model.Id);

                if (fertilizer == null)
                {
                    _logger.LogInformation("Failed to edit fertilizer image - fertilizer not found");
                    return NotFound();
                }

                var extension = Path.GetExtension(model.ImageFile.FileName);

                await _fileRepository.SaveFileAsync(model.ImageFile, 
                    Core.Enums.ImageType.Fertilizer, $"{fertilizer.Id}{extension}");
                fertilizer.ImageExtension = extension;

                await _fertilizerRepository.UpdateAsync(fertilizer);
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
        // must be authorized as an admin
        public async Task<IActionResult> DeleteFertilizer(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to delete fertilizer - id not received");
                return BadRequest();
            }

            try
            {
                var fertilizer = await _fertilizerRepository.GetByIdAsync(id);

                if (fertilizer == null)
                {
                    _logger.LogInformation("Failed to delete fertilizer - fertilizer not found");
                    return NotFound();
                }

                if (fertilizer.ImageExtension != null)
                {
                    await _fileRepository.DeleteFileAsync(Core.Enums.ImageType.Fertilizer,
                        $"{fertilizer.Id}{fertilizer.ImageExtension}");
                }

                await _fertilizerRepository.DeleteAsync(fertilizer);
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
        // must be authorized
        public async Task<IActionResult> GetFertilizer(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to retrieve fertilizer - id not received");
                return BadRequest();
            }

            try
            {
                var fertilizer = await _fertilizerRepository.GetByIdAsync(id);

                if (fertilizer == null)
                {
                    _logger.LogInformation("Failed to retrieve fertilizer - fertilizer not found");
                    return NotFound();
                }

                var model = new FertilizerModel()
                {
                    Name = fertilizer.Name,
                    Description = fertilizer.Description,
                    Price = fertilizer.Price,
                    Size = fertilizer.Size,
                    Ph = fertilizer.Ph
                };

                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to check, if the fertilizer is in cart - user not found");
                }
                else
                {
                    model.IsInCart = user.CartItems.Any(i => i.FertilizerId == fertilizer.Id);
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("image/{id}")]
        // must be authorized
        public async Task<IActionResult> GetFertilizerImage(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to retrieve fertilizer image - id not received");
                return BadRequest();
            }

            try
            {
                var fertilizer = await _fertilizerRepository.GetByIdAsync(id);

                if (fertilizer == null)
                {
                    _logger.LogInformation("Failed to retrieve fertilizer image - plant not found");
                    return NotFound();
                }

                string fileName = $"{fertilizer.Id}{fertilizer.ImageExtension}";
                var file = await _fileRepository.ReadFileAsync(Core.Enums.ImageType.Fertilizer, fileName);

                if (file == null)
                {
                    return NotFound();
                }

                return File(file, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        // must be authorized
        public async Task<IActionResult> GetFertilizers()
        {
            try
            {
                var fertilizers = await _fertilizerRepository.GetAsync();

                if (fertilizers == null)
                {
                    _logger.LogInformation("Failed to retrieve fertilizers - fertilizers not found");
                    return NotFound();
                }

                var models = fertilizers
                    .Select(f => new ListItemModel()
                    {
                        Id = f.Id,
                        Name = f.Name
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("cart")]
        // must be authorized
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve cart items - user not found");
                    return Unauthorized();
                }

                var models = user.CartItems
                    .Select(ci => new ListItemModel()
                    {
                        Id = ci.Fertilizer.Id,
                        Name = ci.Fertilizer.Name
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("cart-android")]
        // must be authorized
        public async Task<IActionResult> GetCartItemsAndroid(string userId)
        {
            try
            {
                var cartItems = await _cartItemRepository.GetAsync(ci => ci.AccountId == userId);

                if (cartItems == null)
                {
                    _logger.LogInformation("Failed to retrieve cart items - cart items not found");
                    return NotFound();
                }

                var models = cartItems
                    .Select(ci => new ListItemModel()
                    {
                        Id = ci.Fertilizer.Id,
                        Name = ci.Fertilizer.Name
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add-to-cart")]
        // must be authorized
        public async Task<IActionResult> AddFertilizerToCart(int id)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve cart items - user not found");
                    return Unauthorized();
                }

                var existingCartItem = await _cartItemRepository.GetByIdAsync(user.Id, id);
                if (existingCartItem != null)
                {
                    _logger.LogInformation("Fertilizer is already in cart");
                    return BadRequest();
                }

                var cartItem = new CartItem()
                {
                    AccountId = user.Id,
                    FertilizerId = id
                };

                await _cartItemRepository.AddAsync(cartItem);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add-to-cart-android")]
        // will be accessed from mobile app
        public async Task<IActionResult> AddFertilizerToCart(string userId, int fertilizerId)
        {
            try
            {
                var existingCartItem = await _cartItemRepository
                    .GetByIdAsync(userId, fertilizerId);
                if (existingCartItem != null)
                {
                    _logger.LogInformation("Fertilizer is already in cart");
                    return BadRequest();
                }

                var cartItem = new CartItem()
                {
                    AccountId = userId,
                    FertilizerId = fertilizerId
                };

                await _cartItemRepository.AddAsync(cartItem);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("delete-from-cart")]
        // must be authorized
        public async Task<IActionResult> DeleteFertilizerFromCart(int id)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to delete cart items - user not found");
                    return Unauthorized();
                }

                var cartItem = await _cartItemRepository.GetByIdAsync(user.Id, id);
                if (cartItem == null)
                {
                    _logger.LogInformation("Fertilizer is not in cart");
                    return BadRequest();
                }

                await _cartItemRepository.DeleteAsync(cartItem);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("delete-from-cart-android")]
        // will be accessed from mobile app
        public async Task<IActionResult> DeleteFertilizerFromCart(string userId, int fertilizerId)
        {
            try
            {
                var cartItem = await _cartItemRepository.GetByIdAsync(userId, fertilizerId);
                if (cartItem == null)
                {
                    _logger.LogInformation("Fertilizer is not in cart");
                    return BadRequest();
                }

                await _cartItemRepository.DeleteAsync(cartItem);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit-cart-item")]
        // must be authorized
        public async Task<IActionResult> EditCartItme([FromBody] EditCartItmeModel model)
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to delete cart items - user not found");
                    return Unauthorized();
                }

                var cartItem = await _cartItemRepository.GetByIdAsync(user.Id, model.FertilizerId);
                if (cartItem == null)
                {
                    _logger.LogInformation("Fertilizer is not in cart");
                    return BadRequest();
                }

                cartItem.Remind = model.Remind;
                cartItem.RemindAt = model.RemindAt;

                await _cartItemRepository.UpdateAsync(cartItem);
                await _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-cart-notifications-android")]
        // will be accessible from android
        public async Task<IActionResult> GetNotificationsAndroid(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve cart notifications - user not found");
                    return Unauthorized();
                }

                // go through cart and find those, that require notification

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
