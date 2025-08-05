using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebNovels.Models;

namespace WebNovels.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Novel> Novels { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Novel>()
                .HasMany(n => n.Genres)
                .WithMany(g => g.Novels);

            builder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Fantasy" },
                new Genre { Id = 2, Name = "Romance" },
                new Genre { Id = 3, Name = "Sci-Fi" },
                new Genre { Id = 4, Name = "Mystery" }
            );

            builder.Entity<Comment>()
                .HasOne(c => c.Chapter)
                .WithMany()
                .HasForeignKey(c => c.ChapterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Bookmark>()
                .HasOne(b => b.Chapter)
                .WithMany()
                .HasForeignKey(b => b.ChapterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Bookmark>()
                .HasOne(b => b.Novel)
                .WithMany()
                .HasForeignKey(b => b.NovelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Bookmark>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
