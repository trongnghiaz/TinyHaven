namespace TinyHaven.Api.Dtos
{
    public class DiaryDtos
    {
        public record CreateDiaryRequest(string Title, string Content, string? Mood);
        public record UpdateDiaryRequest(string Title, string Content, string? Mood);
    }
}
