using Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class FileRepository
    {
        private readonly ILogger<FileRepository> _logger;
        private readonly Dictionary<ImageType, string> _folders = new()
        {
            { ImageType.Plant, "Plants" },
            { ImageType.Fertilizer, "Fertilizers" }
        };
        private readonly string _repositoryPath;

        public FileRepository(ILogger<FileRepository> logger, string repositoryPath)
        {
            _logger = logger;
            _repositoryPath = repositoryPath;
        }

        public async Task SaveFileAsync(IFormFile file, ImageType type, string fileName)
        {
            try
            {
                _logger.LogInformation("Saving file {fileName}", fileName);

                var path = Path.Combine(_repositoryPath, _folders[type]);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                _logger.LogInformation("File was saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save file {fileName}! Error: {errorMessage}",
                    fileName, ex.Message);

                throw new Exception($"Exception message:{ex.Message}");
            }
        }

        public async Task DeleteFileAsync(ImageType type, string fileName)
        {
            try
            {
                _logger.LogInformation("Deleting file {fileName}", fileName);

                var path = Path.Combine(_repositoryPath, _folders[type], fileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                _logger.LogInformation("File was deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete file {fileName}! Error: {errorMessage}",
                    fileName, ex.Message);

                throw new Exception($"Exception message:{ex.Message}");
            }
        }

        public async Task<byte[]> ReadFileAsync(ImageType type, string fileName)
        {
            try
            {
                _logger.LogInformation("Retrieving file {fileName}", fileName);

                var path = Path.Combine(_repositoryPath, _folders[type]);

                if (!Directory.Exists(path))
                {
                    return null;
                }

                var filePath = Path.Combine(path, fileName);
                if (!File.Exists(filePath))
                {
                    return null;
                }

                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var ms = new MemoryStream();

                stream.CopyTo(ms);

                _logger.LogInformation("File was retrieved successfully");

                return ms.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to retrieve file {fileName}! Error: {errorMessage}",
                    fileName, ex.Message);

                throw new Exception($"Exception message:{ex.Message}");
            }
        }
    }
}
