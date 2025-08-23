using Microsoft.AspNetCore.Components.Forms;
using WebNovels.Models;

namespace WebNovels.Services.NovelServices
{
    public interface INovelService
    {
        Task<List<Novel>> GetAllNovelsAsync(string? searchQuery, int page, int pageSize);
        Task<int> GetTotalNovelsCountAsync(string? searchQuery);

        Task<Novel?> GetNovelByIdAsync(int id, string? currentUserId = null);
        Task<Novel?> GetEditableNovelByIdAsync(int id, string currentUserId);
        Task<bool> DeleteNovelAsync(int id, string currentUserId);

        Task<List<Genre>> GetAllGenresAsync();
        Task<List<Novel>> SearchNovelsAsync(string? title, string? author, List<int> genreIds, string sort, int page, int pageSize);
        Task<int> GetSearchNovelsCountAsync(string? title, string? author, List<int> genreIds);

        Task<(bool success, string? errorMessage)> CreateNovelAsync(Novel novel, IBrowserFile? coverImage, List<int> genreIds, string authorId);

        Task<bool> UpdateNovelAsync(Novel novel, IBrowserFile? newCoverImage, List<int> genreIds, string authorId);

        Task<List<Novel>> GetMostReadNovelsAsync(int take);

        Task RecordChapterViewAsync(string userId, int novelId, int chapterId, DateOnly day);

        Task<List<(Novel Novel, int ReadCount)>> GetTrendingNovelsAsync(int days, int take);
    }
}
