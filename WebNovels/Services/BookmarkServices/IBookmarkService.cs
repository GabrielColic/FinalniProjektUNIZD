using WebNovels.Models;

namespace WebNovels.Services.BookmarkServices
{
    public interface IBookmarkService
    {
        Task<Bookmark?> GetBookmarkAsync(string userId, int novelId);
        Task SetBookmarkAsync(string userId, int novelId, int chapterId);
        Task RemoveBookmarkAsync(string userId, int novelId);
        Task<bool> IsChapterBookmarkedAsync(string userId, int chapterId);
        Task<List<Bookmark>> GetAllBookmarksAsync(string userId);

    }
}
