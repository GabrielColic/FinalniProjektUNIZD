using Microsoft.EntityFrameworkCore;
using WebNovels.Data;
using WebNovels.Models;

namespace WebNovels.Services.ChapterServices
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentsForChapterAsync(int chapterId)
        {
            return await _context.Comments
                .Where(c => c.ChapterId == chapterId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddCommentAsync(string userId, int chapterId, string content)
        {
            var comment = new Comment
            {
                UserId = userId,
                ChapterId = chapterId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(int commentId, string userId, string newContent)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment != null)
            {
                comment.Content = newContent;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

    }
}
