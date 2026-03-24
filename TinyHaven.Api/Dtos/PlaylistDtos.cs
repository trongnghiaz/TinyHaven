namespace TinyHaven.Api.Dtos
{
    public class PlaylistDtos
    {
        public record CreatePlaylistRequest(string Title);
        public record AddTrackRequest(string Title, string? Artist, string SourceUrl, int SortOrder, int? DurationSeconds);
        public record UpdateTrackRequest(string Title, string? Artist, string SourceUrl, int SortOrder, int? DurationSeconds);

    }
}
