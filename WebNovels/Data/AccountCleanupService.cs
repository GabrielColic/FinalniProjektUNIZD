using Microsoft.EntityFrameworkCore;

namespace WebNovels.Data
{
    public sealed class AccountCleanupService
    {
        private readonly ApplicationDbContext _db;
        public AccountCleanupService(ApplicationDbContext db) => _db = db;

        public async Task CleanupUserActivityAsync(string userId, CancellationToken ct = default)
        {
            var novelIdsQuery = _db.Novels
                .Where(n => n.AuthorId == userId)
                .Select(n => n.Id);

            var chapterIdsQuery = _db.Chapters
                .Where(c => novelIdsQuery.Contains(c.NovelId))
                .Select(c => c.Id);

            await _db.ChapterDailyViews
                .Where(v => novelIdsQuery.Contains(v.NovelId))
                .ExecuteDeleteAsync(ct);

            await _db.ChapterDailyViews
                .Where(v => chapterIdsQuery.Contains(v.ChapterId))
                .ExecuteDeleteAsync(ct);

            await _db.Notifications
                .Where(n => novelIdsQuery.Contains(n.NovelId))
                .ExecuteDeleteAsync(ct);

            await _db.Comments
                .Where(c => c.UserId == userId)
                .ExecuteDeleteAsync(ct);

            await _db.Reviews
                .Where(r => r.UserId == userId)
                .ExecuteDeleteAsync(ct);

            await _db.Bookmarks
                .Where(b => b.UserId == userId)
                .ExecuteDeleteAsync(ct);

            await _db.Notifications
                .Where(n => n.UserId == userId)
                .ExecuteDeleteAsync(ct);
        }
    }
}
