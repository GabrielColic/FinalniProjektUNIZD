namespace WebNovels.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public int Depth { get; set; } = 0;
        public int RootCommentId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

}
