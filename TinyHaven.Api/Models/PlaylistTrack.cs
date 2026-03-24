namespace TinyHaven.Api.Models
{
    public class PlaylistTrack
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Artist { get; set; }
        public string SourceUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public int? DurationSeconds { get; set; }

    }
}
