namespace WebNovels.Models
{
    public class Bookmark
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public int NovelId { get; set; }
        public Novel Novel { get; set; } = default!;

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
