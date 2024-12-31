using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DatabaseAdminRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<DatabaseAdminRepository> _logger;

        public DatabaseAdminRepository(ApplicationContext context, ILogger<DatabaseAdminRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BackupDatabaseAsync(string backupPath)
        {
            try
            {
                var sql = $@"
                USE master;
                BACKUP DATABASE [GardeningAdviceSystem]
                TO DISK = @backupPath
                WITH FORMAT, INIT;";

                await _context.Database.ExecuteSqlRawAsync(sql, 
                    new[] { new SqlParameter("@backupPath", backupPath) });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to backup the database.", ex);
            }
        }

        public async Task RestoreDatabaseAsync(string backupPath)
        {
            try
            {
                // Check if the database exists
                var checkDatabaseExistsSql = $@"
                USE master;
                SELECT COUNT(*) 
                FROM sys.databases 
                WHERE name = 'GardeningAdviceSystem'";

                var databaseExists = await _context.Database.ExecuteSqlRawAsync(checkDatabaseExistsSql) > 0;

                if (databaseExists)
                {
                    // Disable connections to the database
                    var disableConnectionsSql = $@"
                    USE master;
                    ALTER DATABASE [GardeningAdviceSystem]
                    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    await _context.Database.ExecuteSqlRawAsync(disableConnectionsSql);
                }

                // Restore the database
                var restoreSql = $@"
                USE master;
                RESTORE DATABASE [GardeningAdviceSystem]
                FROM DISK = @backupPath
                WITH REPLACE;";
                await _context.Database.ExecuteSqlRawAsync(restoreSql, 
                    new[] { new SqlParameter("@backupPath", backupPath) });

                if (databaseExists)
                {
                    // Enable connections to the database
                    var enableConnectionsSql = $@"
                    USE master;
                    ALTER DATABASE [GardeningAdviceSystem]
                    SET MULTI_USER;";
                    await _context.Database.ExecuteSqlRawAsync(enableConnectionsSql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to restore the database.", ex);
            }
        }
    }
}
