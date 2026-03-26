namespace TinyHaven.Api.Models
{
    public class Playlist
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public Guid CreatedById { get; set; }
        public AppUser? CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<PlaylistTrack> Tracks { get; set; } = new List<PlaylistTrack>();

    }
}
