using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebNovels.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<Novel> Novels { get; set; } = new List<Novel>();
    }
}
