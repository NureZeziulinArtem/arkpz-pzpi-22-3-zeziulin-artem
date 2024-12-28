using Core.Models;
using DAL;
using GardeningAdviceSystem.Models;
using GardeningAdviceSystem.Models.Plant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GardeningAdviceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Repository<Plant> _plantRepository;
        private readonly FileRepository _fileRepository;
        private readonly UserManager<Account> _userManager;
        private readonly ILogger<PlantController> _logger;

        public PlantController(UnitOfWork unitOfWork, FileRepository fileRepository, 
            UserManager<Account> userManager, ILogger<PlantController> logger)
        {
            _unitOfWork = unitOfWork;
            _plantRepository = unitOfWork.PlantRepository;
            _fileRepository = fileRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        // must be authorized
        public async Task<IActionResult> CreatePlant(CreatePlantModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to create plant - model not received");
                return BadRequest();
            }

            var plant = new Plant()
            {
                Name = model.Name,
                Description = model.Description,
                MinMoisture = model.MinMoisture,
                MaxMoisture = model.MaxMoisture,
                MinPh = model.MinPh,
                MaxPh = model.MaxPh
            };

            // TODO: depending on the user's role the user Id for plant
            // is either saved (normal user) or not saved (admin)

            try
            {
                await _plantRepository.AddAsync(plant);
                await _unitOfWork.Save();

                return Ok(plant.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("edit")]
        // must be authorized
        public async Task<IActionResult> EditPlant(EditPlantModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to edit plant - model not received");
                return BadRequest();
            }

            try
            {
                var plant = await _plantRepository.GetByIdAsync(model.Id);

                if (plant == null)
                {
                    _logger.LogInformation("Failed to edit plant - plant not found");
                    return NotFound();
                }

                plant.Name = model.Name;
                plant.Description = model.Description;
                plant.MinMoisture = model.MinMoisture;
                plant.MaxMoisture = model.MaxMoisture;
                plant.MinPh = model.MinPh;
                plant.MaxPh = model.MaxPh;


                await _plantRepository.UpdateAsync(plant);
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
        // must be authorized
        public async Task<IActionResult> EditPlantImage(EditImageModel model)
        {
            if (model == null)
            {
                _logger.LogError("Failed to edit plant image - model not received");
                return BadRequest();
            }

            try
            {
                var plant = await _plantRepository.GetByIdAsync(model.Id);

                if (plant == null)
                {
                    _logger.LogInformation("Failed to edit plant image - plant not found");
                    return NotFound();
                }

                var extension = Path.GetExtension(model.ImageFile.FileName);

                await _fileRepository.SaveFileAsync(model.ImageFile, 
                    Core.Enums.ImageType.Plant, $"{plant.Id}{extension}");
                plant.ImageExtension = extension;

                await _plantRepository.UpdateAsync(plant);
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
        public async Task<IActionResult> DeletePlant(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to delete plant - id not received");
                return BadRequest();
            }

            try
            {
                var plant = await _plantRepository.GetByIdAsync(id);

                if (plant == null)
                {
                    _logger.LogInformation("Failed to delete plant - plant not found");
                    return NotFound();
                }

                if (plant.ImageExtension != null)
                {
                    await _fileRepository.DeleteFileAsync(Core.Enums.ImageType.Plant, 
                        $"{plant.Id}{plant.ImageExtension}");
                }

                await _plantRepository.DeleteAsync(plant);
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
        public async Task<IActionResult> GetPlant(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to retrieve plant - id not received");
                return BadRequest();
            }

            try
            {
                var plant = await _plantRepository.GetByIdAsync(id);

                if (plant == null)
                {
                    _logger.LogInformation("Failed to retrieve plant - plant not found");
                    return NotFound();
                }

                // check that the plant is created by admin or belongs to current user

                var model = new PlantModel()
                {
                    Name = plant.Name,
                    Description = plant.Description,
                    MaxMoisture = plant.MaxMoisture,
                    MinMoisture = plant.MinMoisture,
                    MaxPh = plant.MaxPh,
                    MinPh = plant.MinPh
                };

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
        public async Task<IActionResult> GetPlantImage(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Failed to retrieve plant image - id not received");
                return BadRequest();
            }

            try
            {
                var plant = await _plantRepository.GetByIdAsync(id);

                if (plant == null)
                {
                    _logger.LogInformation("Failed to retrieve plant image - plant not found");
                    return NotFound();
                }

                // check that the plant is created by admin or belongs to current user

                string fileName = $"{plant.Id}{plant.ImageExtension}";
                var file = await _fileRepository.ReadFileAsync(Core.Enums.ImageType.Plant, fileName);

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
        // retrieves all plants, created by admin
        public async Task<IActionResult> GetPlants()
        {
            try
            {
                var plants = await _plantRepository.GetAsync(p => p.AccountId == null);

                if (plants == null)
                {
                    _logger.LogInformation("Failed to retrieve plants - plants not found");
                    return NotFound();
                }

                var models = plants
                    .Select(p => new ListItemModel()
                    {
                        Id = p.Id,
                        Name = p.Name
                    });

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("by-user")]
        // must be authorized
        // retrieves all plants, created by current user
        public async Task<IActionResult> GetPlantsByUser()
        {
            try
            {
                var user = _userManager.Users.FirstOrDefault();
                // will be change to retrieving user from session

                if (user == null)
                {
                    _logger.LogInformation("Failed to retrieve plants - user not found");
                    return Unauthorized();
                }

                var models = user.AddedPlants
                    .Select(p => new ListItemModel()
                    {
                        Id = p.Id,
                        Name = p.Name
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
