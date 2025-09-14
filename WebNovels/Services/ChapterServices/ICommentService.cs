using WebNovels.Models;

namespace WebNovels.Services.ChapterServices
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsForChapterAsync(int chapterId);
        Task<int> AddCommentAsync(string userId, int chapterId, string content, int? parentCommentId = null, int maxDepth = 6);
        Task AddCommentAsync(string userId, int chapterId, string content);
        Task UpdateCommentAsync(int commentId, string userId, string newContent);
        Task DeleteCommentAsync(int commentId, string userId);

        Task<List<CommentService.CommentNode>> GetCommentTreeAsync(int chapterId);
    }


}
