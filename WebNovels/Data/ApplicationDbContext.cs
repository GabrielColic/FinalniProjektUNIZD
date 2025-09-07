using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
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
        public DbSet<ChapterDailyView> ChapterDailyViews { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Novel>()
                .HasMany(n => n.Genres)
                .WithMany(g => g.Novels);

            builder.Entity<Novel>()
                .Property(n => n.ReadCount)
                .HasDefaultValue(0);          

            builder.Entity<Novel>()
                .HasIndex(n => n.ReadCount);

            builder.Entity<Novel>()
                .ToTable(t => t.HasCheckConstraint("CK_Novel_ReadCount_NonNegative", "[ReadCount] >= 0"));

            builder.Entity<Novel>()
                .HasOne(n => n.Author)
                .WithMany()
                .HasForeignKey(n => n.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Chapter>()
                .HasOne(c => c.Novel)
                .WithMany(n => n.Chapters)
                .HasForeignKey(c => c.NovelId)
                .OnDelete(DeleteBehavior.Cascade);


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
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Bookmark>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChapterDailyView>()
                .Property(x => x.Day)
                .HasColumnType("date");

            builder.Entity<ChapterDailyView>()
                .HasIndex(x => new { x.UserId, x.ChapterId, x.Day })
                .IsUnique();

            builder.Entity<ChapterDailyView>()
                .HasOne<Chapter>()
                .WithMany()
                .HasForeignKey(x => x.ChapterId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.Entity<ChapterDailyView>()
                .HasOne<Novel>()
                .WithMany()
                .HasForeignKey(x => x.NovelId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChapterDailyView>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.Novel)
                .WithMany()
                .HasForeignKey(n => n.NovelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>()
                .HasOne(n => n.Chapter)
                .WithMany()
                .HasForeignKey(n => n.ChapterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead, n.CreatedUtc });

            builder.Entity<Bookmark>()
                .HasIndex(b => new { b.UserId, b.NovelId })
                .IsUnique();

            builder.Entity<Review>(e =>
            {
                e.HasIndex(r => new { r.NovelId, r.UserId }).IsUnique();

                e.Property(r => r.Rating).IsRequired();
                e.Property(r => r.Body).HasMaxLength(4000).IsRequired();
                e.Property(r => r.Title).HasMaxLength(120);

                e.HasOne(r => r.Novel)
                    .WithMany(n => n.Reviews)
                    .HasForeignKey(r => r.NovelId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}
