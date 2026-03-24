namespace TinyHaven.Api.Models
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public string RoomKey { get; set; } = "private-room";
        public Guid SenderId { get; set; }
        public AppUsers? Sender { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
