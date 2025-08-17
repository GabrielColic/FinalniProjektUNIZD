using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebNovels.Models
{
    public class Novel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Synopsis { get; set; }

        public string? CoverImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string AuthorId { get; set; }

        public ApplicationUser Author { get; set; }

        public ICollection<Chapter> Chapters { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public int ReadCount { get; set; }

    }
}
