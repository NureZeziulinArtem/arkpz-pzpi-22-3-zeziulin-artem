using Core;
using DAL;
using DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GardeningAdviceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.DbAdmin)]
    public class DatabaseAdminController : ControllerBase
    {
        private readonly DatabaseAdminRepository _dbAdminRepository;
        private readonly ILogger<DatabaseAdminController> _logger;

        public DatabaseAdminController(UnitOfWork unitOfWork, ILogger<DatabaseAdminController> logger)
        {
            _dbAdminRepository = unitOfWork.DatabaseAdminRepository;
            _logger = logger;
        }

        [HttpPost("backup-db")]
        public async Task<IActionResult> BackupDb(string backupPath)
        {
            try
            {
                await _dbAdminRepository.BackupDatabaseAsync(backupPath);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("restore-db")]
        public async Task<IActionResult> RestoreDb(string backupPath)
        {
            try
            {
                await _dbAdminRepository.RestoreDatabaseAsync(backupPath);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
