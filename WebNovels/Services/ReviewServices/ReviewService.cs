namespace WebNovels.Services.ReviewServices
{
    using Microsoft.EntityFrameworkCore;
    using WebNovels.Data; // adjust if your DbContext namespace differs
    using WebNovels.Models;

    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _db;

        public ReviewService(ApplicationDbContext db) => _db = db;

        public async Task<(IReadOnlyList<Review> Items, int Total)> GetForNovelAsync(
            int novelId, string sort, int page, int pageSize)
        {
            var q = _db.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.NovelId == novelId);

            q = sort switch
            {
                "highest" => q.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedUtc),
                "lowest" => q.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedUtc),
                "oldest" => q.OrderBy(r => r.CreatedUtc),
                _ => q.OrderByDescending(r => r.CreatedUtc) // "newest"
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, total);
        }

        public async Task<(double Average, int Count)> GetSummaryAsync(int novelId)
        {
            var q = _db.Reviews.Where(r => r.NovelId == novelId);
            var count = await q.CountAsync();
            if (count == 0) return (0, 0);
            var avg = await q.AverageAsync(r => (double)r.Rating);
            return (Math.Round(avg, 1), count);
        }

        public async Task<Review?> GetUserReviewAsync(int novelId, string userId)
        {
            return await _db.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.NovelId == novelId && r.UserId == userId);
        }

        public async Task<Review> CreateOrUpdateAsync(
            int novelId, string userId, int rating, string? title, string body)
        {
            rating = Math.Clamp(rating, 1, 5);

            var existing = await _db.Reviews
                .FirstOrDefaultAsync(r => r.NovelId == novelId && r.UserId == userId);

            if (existing is null)
            {
                var r = new Review
                {
                    NovelId = novelId,
                    UserId = userId,
                    Rating = rating,
                    Title = string.IsNullOrWhiteSpace(title) ? null : title!.Trim(),
                    Body = body.Trim(),
                    CreatedUtc = DateTime.UtcNow
                };

                _db.Reviews.Add(r);
                await _db.SaveChangesAsync();
                return r;
            }
            else
            {
                existing.Rating = rating;
                existing.Title = string.IsNullOrWhiteSpace(title) ? null : title!.Trim();
                existing.Body = body.Trim();
                existing.UpdatedUtc = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                return existing;
            }
        }

        public async Task<bool> DeleteAsync(int reviewId, string userId)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review is null || review.UserId != userId) return false;
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<int, (double Average, int Count)>> GetSummariesAsync(IEnumerable<int> novelIds)
        {
            var ids = novelIds.Distinct().ToList();
            if (ids.Count == 0) return new();

            var rows = await _db.Reviews
                .AsNoTracking()
                .Where(r => ids.Contains(r.NovelId))
                .GroupBy(r => r.NovelId)
                .Select(g => new
                {
                    NovelId = g.Key,
                    Count = g.Count(),
                    Average = Math.Round(g.Average(r => (double)r.Rating), 1, MidpointRounding.AwayFromZero)
                })
                .ToListAsync();

            return rows.ToDictionary(x => x.NovelId, x => (x.Average, x.Count));
        }

    }
}
