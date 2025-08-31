using System;

namespace WebNovels.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public int NovelId { get; set; }
        public Novel Novel { get; set; } = default!;

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; } = default!;

        public string Message { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
