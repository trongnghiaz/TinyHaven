using Microsoft.EntityFrameworkCore;
using TinyHaven.Api.Models;

namespace TinyHaven.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Models.CountdownEvent> CountdownEvents => Set<Models.CountdownEvent>();
        public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();
        public DbSet<AlbumPhoto> AlbumPhotos => Set<AlbumPhoto>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(e =>
            {
                e.ToTable("app_users");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Username).HasMaxLength(50).IsRequired();
                e.Property(x => x.PasswordHash).IsRequired();
                e.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
                e.Property(x => x.AvatarUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<Models.CountdownEvent>(e =>
            {
                e.ToTable("countdown_events");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(150).IsRequired();
                e.Property(x => x.Description).HasMaxLength(1000);
                e.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .HasForeignKey(x => x.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DiaryEntry>(e =>
            {
                e.ToTable("diary_entries");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Content).IsRequired();
                e.Property(x => x.Mood).HasMaxLength(50);
                e.HasOne(x => x.CreatedBy)
                    .WithMany(x => x.DiaryEntries)
                    .HasForeignKey(x => x.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AlbumPhoto>(e =>
            {
                e.ToTable("album_photos");
                e.HasKey(x => x.Id);
                e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
                e.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
                e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
                e.Property(x => x.Url).HasMaxLength(500).IsRequired();
                e.Property(x => x.Caption).HasMaxLength(500);
                e.HasOne(x => x.UploadedBy)
                    .WithMany(x => x.AlbumPhotos)
                    .HasForeignKey(x => x.UploadedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatMessage>(e =>
            {
                e.ToTable("chat_messages");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.RoomKey).HasMaxLength(50).IsRequired();
                e.Property(x => x.Message).HasMaxLength(4000).IsRequired();
                e.HasOne(x => x.Sender)
                    .WithMany()
                    .HasForeignKey(x => x.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Playlist>(e =>
            {
                e.ToTable("playlists");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(150).IsRequired();
                e.HasOne(x => x.CreatedBy)
                    .WithMany(x => x.Playlists)
                    .HasForeignKey(x => x.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PlaylistTrack>(e =>
            {
                e.ToTable("playlist_tracks");
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Artist).HasMaxLength(200);
                e.Property(x => x.SourceUrl).HasMaxLength(1000).IsRequired();
                e.HasOne(x => x.Playlist)
                    .WithMany(x => x.Tracks)
                    .HasForeignKey(x => x.PlaylistId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
