using Microsoft.EntityFrameworkCore;
using WebNovels.Data;
using WebNovels.Models;

namespace WebNovels.Services.BookmarkServices
{
    public class BookmarkService : IBookmarkService
    {
        private readonly ApplicationDbContext _context;

        public BookmarkService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bookmark?> GetBookmarkAsync(string userId, int novelId)
        {
            return await _context.Bookmarks
                .Include(b => b.Chapter)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.NovelId == novelId);
        }

        public async Task SetBookmarkAsync(string userId, int novelId, int chapterId)
        {
            var existing = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.NovelId == novelId);

            if (existing != null)
            {
                existing.ChapterId = chapterId;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                var bookmark = new Bookmark
                {
                    UserId = userId,
                    NovelId = novelId,
                    ChapterId = chapterId
                };

                _context.Bookmarks.Add(bookmark);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveBookmarkAsync(string userId, int novelId)
        {
            var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.NovelId == novelId);
            if (bookmark != null)
            {
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsChapterBookmarkedAsync(string userId, int chapterId)
        {
            return await _context.Bookmarks.AnyAsync(b => b.UserId == userId && b.ChapterId == chapterId);
        }

        public async Task<List<Bookmark>> GetAllBookmarksAsync(string userId)
        {
            return await _context.Bookmarks
                .Include(b => b.Novel)
                    .ThenInclude(n => n.Chapters)
                .Include(b => b.Chapter) 
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }



    }
}
