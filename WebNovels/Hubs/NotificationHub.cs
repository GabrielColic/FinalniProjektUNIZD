using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebNovels.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // await _hubContext.Clients.Users(userIds).SendAsync("NewNotification", payload);
    }
}
