using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using System.IO;

namespace RoomRentalManagerServer.API.Services
{
    public class LocalFileStorageAppService : ILocalFileStorageAppService
    {
        private readonly ILogger<LocalFileStorageAppService> _logger;
        public LocalFileStorageAppService(ILogger<LocalFileStorageAppService> logger)
        {
            _logger = logger;
        }

        public async Task<(List<string> Paths, List<string> Errors)> UploadFilesAsync(IEnumerable<IFormFile> files, string relativeFolder, string webRootPath)
        {
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif" };
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB

            var savedPaths = new List<string>();
            var errors = new List<string>();

            var webRoot = webRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsRootFolder = Path.Combine(webRoot, relativeFolder.Trim('/', '\\'));
            if (!Directory.Exists(uploadsRootFolder))
                Directory.CreateDirectory(uploadsRootFolder);

            foreach (var file in files)
            {
                if (file == null)
                    continue;

                if (file.Length == 0)
                {
                    errors.Add($"File {file?.FileName} is empty.");
                    continue;
                }

                if (file.Length > maxFileSize)
                {
                    errors.Add($"File {file.FileName} exceeds the allowed size of {maxFileSize} bytes.");
                    continue;
                }

                var ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
                {
                    errors.Add($"File {file.FileName} has an invalid extension.");
                    continue;
                }

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                try
                {
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var relativePath = $"/{relativeFolder.Trim('/')}/{uniqueFileName}";
                    savedPaths.Add(relativePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save uploaded image {FileName}", file.FileName);
                    errors.Add($"Failed to save {file.FileName}: {ex.Message}");
                }
            }

            return (savedPaths, errors);
        }

        public async Task<(string Path, string? Error)> UploadFileAsync(IFormFile file, string relativeFolder, string webRootPath)
        {
            var (paths, errors) = await UploadFilesAsync(new[] { file }, relativeFolder, webRootPath);
            return (paths.FirstOrDefault() ?? string.Empty, errors.FirstOrDefault());
        }

        public Task<bool> DeleteFileAsync(string relativePath, string webRootPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath))
                    return Task.FromResult(false);

                var webRoot = webRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var relative = relativePath.TrimStart('~').TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(webRoot, relative);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete file {Path}", relativePath);
                return Task.FromResult(false);
            }
        }
    }
}
