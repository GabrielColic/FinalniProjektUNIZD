namespace WebNovels.Services.ReviewServices
{
    using WebNovels.Models;

    public interface IReviewService
    {
        Task<(IReadOnlyList<Review> Items, int Total)> GetForNovelAsync(
            int novelId, string sort, int page, int pageSize);

        Task<(double Average, int Count)> GetSummaryAsync(int novelId);

        Task<Review?> GetUserReviewAsync(int novelId, string userId);

        Task<Review> CreateOrUpdateAsync(
            int novelId, string userId, int rating, string? title, string body);

        Task<bool> DeleteAsync(int reviewId, string userId);
    }
}
