namespace WebNovels.Models
{
    public class ChapterDailyView
    {
        public int Id { get; set; }
        public string? UserId { get; set; } = default!;
        public int NovelId { get; set; }
        public int ChapterId { get; set; }
        public DateOnly Day { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
