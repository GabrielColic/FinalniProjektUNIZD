using Microsoft.AspNetCore.Components.Forms;

namespace WebNovels.Services.ChapterServices
{
    public interface IContentParserService
    {
        Task<string> ParseAsync(IBrowserFile file);
    }

}
