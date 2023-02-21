using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class User: IdentityUser
    {
        public bool IsConnected { get; set; } = false;
    }
}
