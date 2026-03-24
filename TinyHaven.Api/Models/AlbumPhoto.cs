namespace TinyHaven.Api.Models
{
    public class AlbumPhoto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Caption { get; set; }

        public Guid UploadedById { get; set; }
        public AppUsers? UploadedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
