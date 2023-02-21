using Models;
using System.Security.Claims;

namespace TalkBack.ContactsDB.Services.Token
{
    public interface IJWTTokenGenerator
    {
        //string GenerateToken(User user, IList<string> roles, IList<Claim> claims);
        string GenerateToken(User user);
    }
}
