namespace TinyHaven.Api.Models
{
    public class CountdownEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "Ngày gặp lại";
        public string? Description { get; set; }
        public DateTimeOffset TargetDate { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CreatedById { get; set; }
        public AppUsers? CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

    }
}
