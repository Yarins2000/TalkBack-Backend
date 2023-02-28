using TalkBack.Models;

namespace TalkBack.API.JwtServices
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(User user);
    }
}
