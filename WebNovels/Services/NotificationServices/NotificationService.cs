using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebNovels.Data;
using WebNovels.Hubs;
using WebNovels.Models;

namespace WebNovels.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hub)
        {
            _scopeFactory = scopeFactory;
            _hub = hub;
        }

        public async Task<List<Notification>> GetUnreadAsync(string userId, int take = 20)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await db.Notifications.AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedUtc)
                .Take(take)
                .ToListAsync();
        }

        public async Task MarkReadAsync(int notificationId, string userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.Notifications
                .Where(n => n.Id == notificationId && n.UserId == userId)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task MarkAllReadAsync(string userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task NotifyNewChapterAsync(int novelId, int chapterId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userIds = await db.Bookmarks
                .Where(b => b.NovelId == novelId)
                .Select(b => b.UserId)
                .Distinct()
                .ToListAsync();

            if (userIds.Count == 0) return;

            var novelTitle = await db.Novels.Where(n => n.Id == novelId).Select(n => n.Title).FirstAsync();
            var chapterTitle = await db.Chapters.Where(c => c.Id == chapterId).Select(c => c.Title).FirstAsync();
            var message = $"{novelTitle}: New chapter — {chapterTitle}";

            var notifications = userIds.Select(uid => new Notification
            {
                UserId = uid,
                NovelId = novelId,
                ChapterId = chapterId,
                Message = message,
                CreatedUtc = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            db.Notifications.AddRange(notifications);
            await db.SaveChangesAsync();

            foreach (var n in notifications)
            {
                await _hub.Clients.User(n.UserId).SendAsync("NewNotification", new
                {
                    notificationId = n.Id,
                    novelId,
                    chapterId,
                    message,
                    createdUtc = n.CreatedUtc
                });
            }
        }
    }
}
