using Microsoft.AspNetCore.Identity;

namespace WebNovels.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? PreferredFontSize { get; set; }
        public string? PreferredFontFamily { get; set; }
    }

}
