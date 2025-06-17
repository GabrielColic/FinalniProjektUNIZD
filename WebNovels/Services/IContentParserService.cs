using Microsoft.AspNetCore.Components.Forms;

namespace WebNovels.Services
{
    public interface IContentParserService
    {
        Task<string> ParseAsync(IBrowserFile file);
    }

}
