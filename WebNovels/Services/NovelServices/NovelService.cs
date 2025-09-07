using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebNovels.Data;
using WebNovels.Models;

namespace WebNovels.Services.NovelServices
{
    public class NovelService : INovelService
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileUploadService _fileUploadService;

        public NovelService(ApplicationDbContext db, IFileUploadService fileUploadService)
        {
            _db = db;
            _fileUploadService = fileUploadService;
        }

        public async Task<List<Novel>> GetAllNovelsAsync(string? searchQuery, int page, int pageSize)
        {
            var query = _db.Novels.Include(n => n.Author).Include(n => n.Genres).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(n => n.Title.Contains(searchQuery));
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalNovelsCountAsync(string? searchQuery)
        {
            var query = _db.Novels.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(n => n.Title.Contains(searchQuery));
            }

            return await query.CountAsync();
        }

        public async Task<Novel?> GetNovelByIdAsync(int id, string? currentUserId = null)
        {
            var novel = await _db.Novels
                .Include(n => n.Author)
                .Include(n => n.Chapters)
                .Include(n => n.Genres)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (novel == null) return null;

            if (novel.AuthorId != currentUserId)
            {
                novel.Chapters = novel.Chapters?.Where(c => c.IsPublished).ToList();
            }

            return novel;
        }

        public async Task<Novel?> GetEditableNovelByIdAsync(int id, string currentUserId)
        {
            var novel = await _db.Novels
                .Include(n => n.Author)
                .Include(n => n.Genres)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (novel?.AuthorId != currentUserId) return null;

            return novel;
        }

        public async Task<bool> DeleteNovelAsync(int id, string currentUserId)
        {
            var novelInfo = await _db.Novels
                .Where(n => n.Id == id)
                .Select(n => new { n.AuthorId, n.CoverImagePath })
                .FirstOrDefaultAsync();

            if (novelInfo is null || novelInfo.AuthorId != currentUserId)
                return false;

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var chapterIds = await _db.Chapters
                    .Where(c => c.NovelId == id)
                    .Select(c => c.Id)
                    .ToListAsync();

                await _db.Notifications
                    .Where(n => n.NovelId == id || chapterIds.Contains(n.ChapterId))
                    .ExecuteDeleteAsync();

                await _db.ChapterDailyViews
                    .Where(v => v.NovelId == id || chapterIds.Contains(v.ChapterId))
                    .ExecuteDeleteAsync();

                await _db.Comments
                    .Where(c => chapterIds.Contains(c.ChapterId))
                    .ExecuteDeleteAsync();

                // Optional but harmless even if you keep DB cascade:
                await _db.Bookmarks
                    .Where(b => b.NovelId == id)
                    .ExecuteDeleteAsync();

                // 2) Delete the novel (Chapters and/or Bookmarks can cascade at DB level)
                var deleted = await _db.Novels
                    .Where(n => n.Id == id && n.AuthorId == currentUserId)
                    .ExecuteDeleteAsync();

                if (deleted == 0)
                {
                    await tx.RollbackAsync();
                    return false;
                }

                await tx.CommitAsync();

                // 3) Remove the cover file after DB success
                if (!string.IsNullOrWhiteSpace(novelInfo.CoverImagePath))
                    _fileUploadService.DeleteFile(novelInfo.CoverImagePath);

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                return false;
            }
        }


        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _db.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<List<Novel>> SearchNovelsAsync(string? title, string? author, List<int> genreIds, string sort, int page, int pageSize)
        {
            var query = _db.Novels
                .Include(n => n.Author)
                .Include(n => n.Genres)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(n => n.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(n => n.Author.UserName.Contains(author));

            if (genreIds.Any())
                query = query.Where(n => n.Genres.Any(g => genreIds.Contains(g.Id)));

            query = sort switch
            {
                "oldest" => query.OrderBy(n => n.CreatedAt),
                "title_asc" => query.OrderBy(n => n.Title),
                "title_desc" => query.OrderByDescending(n => n.Title),
                "reads" => query.OrderByDescending(n => n.ReadCount).ThenBy(n => n.Title),
                _ => query.OrderByDescending(n => n.CreatedAt)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetSearchNovelsCountAsync(string? title, string? author, List<int> genreIds)
        {
            var query = _db.Novels
                .Include(n => n.Author)
                .Include(n => n.Genres)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(n => n.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(author))
                query = query.Where(n => n.Author.UserName.Contains(author));

            if (genreIds.Any())
                query = query.Where(n => n.Genres.Any(g => genreIds.Contains(g.Id)));

            return await query.CountAsync();
        }

        public async Task<(bool success, string? errorMessage)> CreateNovelAsync(Novel novel, IBrowserFile? coverImage, List<int> genreIds, string authorId)
        {
            novel.AuthorId = authorId;

            if (coverImage != null)
            {
                var (success, path, error) = await _fileUploadService.UploadCoverImageAsync(coverImage);
                if (!success)
                    return (false, error);

                novel.CoverImagePath = path;
            }

            novel.Genres = await _db.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

            _db.Novels.Add(novel);
            await _db.SaveChangesAsync();

            return (true, null);
        }


        public async Task<bool> UpdateNovelAsync(Novel novel, IBrowserFile? newCoverImage, List<int> genreIds, string authorId)
        {
            var existing = await _db.Novels.Include(n => n.Genres).FirstOrDefaultAsync(n => n.Id == novel.Id);

            if (existing == null || existing.AuthorId != authorId)
                return false;

            existing.Title = novel.Title;
            existing.Synopsis = novel.Synopsis;

            if (newCoverImage != null)
            {
                var (success, path, _) = await _fileUploadService.UploadCoverImageAsync(newCoverImage, existing.CoverImagePath);
                if (!success) return false;

                existing.CoverImagePath = path;
            }

            existing.Genres = await _db.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

            _db.Novels.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Novel>> GetMostReadNovelsAsync(int take)
        {
            return await _db.Novels
                .AsNoTracking()
                .Include(n => n.Author)
                .Include(n => n.Genres)
                .OrderByDescending(n => n.ReadCount)
                .ThenBy(n => n.Title)
                .Take(take)
                .ToListAsync();
        }

        public async Task RecordChapterViewAsync(string userId, int novelId, int chapterId, DateOnly day)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var view = new ChapterDailyView
                {
                    UserId = userId,
                    NovelId = novelId,
                    ChapterId = chapterId,
                    Day = day,
                    CreatedUtc = DateTime.UtcNow
                };
                _db.ChapterDailyViews.Add(view);

                await _db.SaveChangesAsync();

                await _db.Novels
                    .Where(n => n.Id == novelId)
                    .ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadCount, n => n.ReadCount + 1));

                await tx.CommitAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                await tx.RollbackAsync();

                foreach (var e in _db.ChangeTracker.Entries<ChapterDailyView>()
                                     .Where(e => e.State == EntityState.Added
                                              && e.Entity.UserId == userId
                                              && e.Entity.ChapterId == chapterId
                                              && e.Entity.Day == day)
                                     .ToList())
                {
                    e.State = EntityState.Detached;
                }
            }
        }


        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;

            return false;
        }

        public async Task<List<(Novel Novel, int ReadCount)>> GetTrendingNovelsAsync(int days, int take)
        {
            var since = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-days));

            var top = await _db.ChapterDailyViews
                .Where(v => v.Day >= since)
                .GroupBy(v => v.NovelId)
                .Select(g => new { NovelId = g.Key, ReadCount = g.Count() })
                .OrderByDescending(x => x.ReadCount)
                .ThenBy(x => x.NovelId)
                .Take(take)
                .ToListAsync();

            if (top.Count == 0) return new();

            var ids = top.Select(t => t.NovelId).ToList();

            var novels = await _db.Novels
                .AsNoTracking()
                .Include(n => n.Author)
                .Include(n => n.Genres)
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();

            var byId = novels.ToDictionary(n => n.Id);

            return top
                .Where(t => byId.ContainsKey(t.NovelId))
                .Select(t => (byId[t.NovelId], t.ReadCount))
                .ToList();
        }

    }
}
