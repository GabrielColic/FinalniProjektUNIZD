using Microsoft.AspNetCore.Components.Forms;
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
            var novel = await _db.Novels.Include(n => n.Author).FirstOrDefaultAsync(n => n.Id == id);

            if (novel == null || novel.AuthorId != currentUserId)
                return false;

            if (!string.IsNullOrEmpty(novel.CoverImagePath))
            {
                _fileUploadService.DeleteFile(novel.CoverImagePath);
            }

            _db.Novels.Remove(novel);
            await _db.SaveChangesAsync();
            return true;
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
    }
}
