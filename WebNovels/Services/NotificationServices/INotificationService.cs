using WebNovels.Models;

namespace WebNovels.Services.NotificationServices
{
    public interface INotificationService
    {
        Task NotifyNewChapterAsync(int novelId, int chapterId);
        Task<List<Notification>> GetUnreadAsync(string userId, int take = 20);
        Task MarkAllReadAsync(string userId);
        Task MarkReadAsync(int notificationId, string userId);
    }
}
