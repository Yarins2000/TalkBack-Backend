using Microsoft.AspNetCore.Identity;

namespace TalkBack.Models
{
    public class User: IdentityUser
    {
        public bool IsConnected { get; set; } = false;
    }
}
