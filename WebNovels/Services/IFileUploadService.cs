using Microsoft.AspNetCore.Components.Forms;

namespace WebNovels.Services
{
    public interface IFileUploadService
    {
        Task<(bool success, string? path, string? error)> UploadCoverImageAsync(IBrowserFile file, string? oldPath = null);

        public void DeleteFile(string? relativePath);
    }

}
