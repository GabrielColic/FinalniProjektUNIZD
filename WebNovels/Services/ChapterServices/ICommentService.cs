using WebNovels.Models;

namespace WebNovels.Services.ChapterServices
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsForChapterAsync(int chapterId);
        Task AddCommentAsync(string userId, int chapterId, string content);
        Task UpdateCommentAsync(int commentId, string userId, string newContent);
        Task DeleteCommentAsync(int commentId, string userId);
    }
}
