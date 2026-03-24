namespace TinyHaven.Api.Dtos
{
    public class AuthDtos
    {
        public record LoginRequest(string Username, string Password);
    }
}
