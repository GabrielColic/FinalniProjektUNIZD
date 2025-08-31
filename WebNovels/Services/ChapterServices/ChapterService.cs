using Microsoft.EntityFrameworkCore;
using WebNovels.Data;
using WebNovels.Models;
using WebNovels.Services.NotificationServices;

namespace WebNovels.Services.ChapterServices
{
    public class ChapterService : IChapterService
    {
        private readonly ApplicationDbContext _db;
        private readonly INotificationService _notifications;

        public ChapterService(ApplicationDbContext db, INotificationService notifications)
        {
            _db = db;
            _notifications = notifications;
        }

        public async Task<Chapter?> GetEditableChapterAsync(int chapterId, string userId)
        {
            var chapter = await _db.Chapters.Include(c => c.Novel)
                .FirstOrDefaultAsync(c => c.Id == chapterId);
            return chapter?.Novel?.AuthorId == userId ? chapter : null;
        }

        public async Task<Chapter?> GetChapterByIdAsync(int chapterId) =>
            await _db.Chapters.Include(c => c.Novel).ThenInclude(n => n.Author)
                .FirstOrDefaultAsync(c => c.Id == chapterId);

        public async Task<List<Chapter>> GetAllVisibleChapters(int novelId, bool includeDrafts) =>
            await _db.Chapters
                .Where(c => c.NovelId == novelId && (c.IsPublished || includeDrafts))
                .OrderBy(c => c.Order)
                .ToListAsync();

        public async Task SaveChapterAsync(Chapter chapter)
        {
            bool firePublishEvent = false;

            if (chapter.Id == 0)
            {
                if (chapter.IsPublished) firePublishEvent = true;
                await _db.Chapters.AddAsync(chapter);
            }
            else
            {
                var existing = await _db.Chapters.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == chapter.Id);

                if (existing == null)
                {
                    if (chapter.IsPublished) firePublishEvent = true;
                    await _db.Chapters.AddAsync(chapter);
                }
                else
                {
                    firePublishEvent = !existing.IsPublished && chapter.IsPublished;
                    _db.Entry(chapter).State = EntityState.Modified;
                }
            }

            await _db.SaveChangesAsync();

            if (firePublishEvent)
                await _notifications.NotifyNewChapterAsync(chapter.NovelId, chapter.Id);
        }

        public async Task DeleteChapterAsync(int chapterId, string userId)
        {
            var chapter = await _db.Chapters.Include(c => c.Novel)
                .FirstOrDefaultAsync(c => c.Id == chapterId && c.Novel!.AuthorId == userId);

            if (chapter != null)
            {
                _db.Chapters.Remove(chapter);
                await _db.SaveChangesAsync();
            }
        }
    }
}
