using System;
using System.ComponentModel.DataAnnotations;

namespace WebNovels.Models
{
    public class Chapter
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public int Order { get; set; }

        public DateTime PublishedAt { get; set; } = DateTime.Now;

        public int NovelId { get; set; }

        public Novel Novel { get; set; }

        public bool IsPublished { get; set; } = true;
    }
}
