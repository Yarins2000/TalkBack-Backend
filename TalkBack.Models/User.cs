using Microsoft.AspNetCore.Identity;

namespace TalkBack.Models
{
    /// <summary>
    /// A class represents a user.
    /// </summary>
    public class User : IdentityUser
    {
        public bool IsConnected { get; set; } = false;
    }
}
