using TinyHaven.Api.Models;

namespace TinyHaven.Api.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(AppUser user);
    }
}
