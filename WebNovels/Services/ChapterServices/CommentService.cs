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

        public Task AddCommentAsync(string userId, int chapterId, string content)
            => AddCommentAsync(userId, chapterId, content, parentCommentId: null);

        public async Task<int> AddCommentAsync(string userId, int chapterId, string content, int? parentCommentId, int maxDepth = 6)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("userId required");
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("content required");

            int depth = 0;
            int rootId = 0;

            if (parentCommentId is int pid)
            {
                var parent = await _context.Comments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == pid && c.ChapterId == chapterId);

                if (parent is null)
                    throw new InvalidOperationException("Parent comment not found.");

                depth = parent.Depth + 1;
                if (depth > maxDepth)
                    throw new InvalidOperationException("Max reply depth reached.");

                rootId = parent.Depth == 0 ? parent.Id : parent.RootCommentId;
            }

            var c = new Comment
            {
                UserId = userId,
                ChapterId = chapterId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow,
                ParentCommentId = parentCommentId,
                Depth = depth,
                RootCommentId = rootId // corrected after save for top-level
            };

            _context.Comments.Add(c);
            await _context.SaveChangesAsync();

            if (c.Depth == 0)
            {
                c.RootCommentId = c.Id;
                _context.Entry(c).Property(x => x.RootCommentId).IsModified = true;
                await _context.SaveChangesAsync();
            }

            return c.Id;
        }

        public async Task UpdateCommentAsync(int commentId, string userId, string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent)) return;

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && !c.IsDeleted);

            if (comment is null) return;

            comment.Content = newContent.Trim();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment is null) return;

            if (comment.Replies.Any())
            {
                comment.IsDeleted = true;
                comment.Content = "[deleted]";
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public sealed class CommentNode
        {
            public int Id { get; init; }
            public int ChapterId { get; init; }
            public int? ParentCommentId { get; init; }
            public int Depth { get; init; }
            public int RootCommentId { get; init; }
            public string Content { get; init; } = "";
            public bool IsDeleted { get; init; }
            public DateTime CreatedAt { get; init; }
            public string? UserId { get; init; }
            public string? UserName { get; init; }
            public List<CommentNode> Replies { get; } = new();
        }


        public async Task<List<CommentNode>> GetCommentTreeAsync(int chapterId)
        {
            var rows = await _context.Comments
                .Where(c => c.ChapterId == chapterId)
                .OrderBy(c => c.RootCommentId).ThenBy(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.ChapterId,
                    c.ParentCommentId,
                    c.Depth,
                    c.RootCommentId,
                    c.Content,
                    c.IsDeleted,
                    c.CreatedAt,
                    UserName = c.User.UserName
                })
                .AsNoTracking()
                .ToListAsync();

            var map = rows.ToDictionary(
                r => r.Id,
                r => new CommentNode
                {
                    Id = r.Id,
                    ChapterId = r.ChapterId,
                    ParentCommentId = r.ParentCommentId,
                    Depth = r.Depth,
                    RootCommentId = r.RootCommentId,
                    Content = r.Content,
                    IsDeleted = r.IsDeleted,
                    CreatedAt = r.CreatedAt,
                    UserName = r.UserName
                });

            var roots = new List<CommentNode>();
            foreach (var r in rows)
            {
                var node = map[r.Id];
                if (r.ParentCommentId is null)
                    roots.Add(node);
                else if (map.TryGetValue(r.ParentCommentId.Value, out var parent))
                    parent.Replies.Add(node);
            }

            return roots;
        }
    }
}
