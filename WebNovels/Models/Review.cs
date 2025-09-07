using System.ComponentModel.DataAnnotations;

namespace WebNovels.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int NovelId { get; set; }
        public Novel Novel { get; set; } = default!;

        [Required]
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(120)]
        public string? Title { get; set; }

        [Required, MaxLength(4000)]
        public string Body { get; set; } = default!;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedUtc { get; set; }
        public bool IsEdited => UpdatedUtc != null;
    }
}
