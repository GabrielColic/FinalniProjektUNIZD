using Microsoft.AspNetCore.Components.Forms;

namespace WebNovels.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(bool success, string? path, string? error)> UploadCoverImageAsync(IBrowserFile file, string? oldPath = null)
        {
            var extension = Path.GetExtension(file.Name)?.ToLowerInvariant().Trim();

            if (!_allowedExtensions.Contains(extension))
            {
                return (false, null, "Only .jpg, .jpeg, and .png files are allowed.");
            }

            if (!string.IsNullOrEmpty(oldPath))
            {
                var fullOldPath = Path.Combine(_env.WebRootPath, oldPath.TrimStart('/'));
                if (File.Exists(fullOldPath))
                {
                    File.Delete(fullOldPath);
                }
            }

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(stream);

            var relativePath = $"/uploads/{fileName}";
            return (true, relativePath, null);
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }

}
