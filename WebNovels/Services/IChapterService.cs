using WebNovels.Models;
namespace WebNovels.Services
{
    public interface IChapterService
    {
        Task<Chapter?> GetEditableChapterAsync(int chapterId, string userId);
        Task<Chapter?> GetChapterByIdAsync(int chapterId);
        Task<List<Chapter>> GetAllVisibleChapters(int novelId, bool includeDrafts);
        Task SaveChapterAsync(Chapter chapter);
        Task DeleteChapterAsync(int chapterId, string userId);
    }

}
