using Microsoft.EntityFrameworkCore;
using WebNovels.Data;
using WebNovels.Models;

namespace WebNovels.Services
{
    public class ChapterService : IChapterService
    {
        private readonly ApplicationDbContext _db;

        public ChapterService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Chapter?> GetEditableChapterAsync(int chapterId, string userId)
        {
            var chapter = await _db.Chapters
                .Include(c => c.Novel)
                .FirstOrDefaultAsync(c => c.Id == chapterId);

            if (chapter?.Novel?.AuthorId != userId)
                return null;

            return chapter;
        }

        public async Task<Chapter?> GetChapterByIdAsync(int chapterId)
        {
            return await _db.Chapters
                .Include(c => c.Novel)
                .ThenInclude(n => n.Author)
                .FirstOrDefaultAsync(c => c.Id == chapterId);
        }

        public async Task<List<Chapter>> GetAllVisibleChapters(int novelId, bool includeDrafts)
        {
            return await _db.Chapters
                .Where(c => c.NovelId == novelId && (c.IsPublished || includeDrafts))
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task SaveChapterAsync(Chapter chapter)
        {
            _db.Chapters.Update(chapter);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteChapterAsync(int chapterId, string userId)
        {
            var chapter = await _db.Chapters
                .Include(c => c.Novel)
                .FirstOrDefaultAsync(c => c.Id == chapterId && c.Novel!.AuthorId == userId);

            if (chapter != null)
            {
                _db.Chapters.Remove(chapter);
                await _db.SaveChangesAsync();
            }
        }
    }
}
