namespace TinyHaven.Api.Dtos
{
    public class CountdownDtos
    {
        public record CountdownUpsertRequest(string Title, string? Description, DateTimeOffset TargetDate);
    }
}
