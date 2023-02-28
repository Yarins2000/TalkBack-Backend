using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalkBack.Models;

namespace TalkBack.Data
{
    /// <summary>
    /// The db context class for the users.
    /// </summary>
    public class ContactsDbContext : IdentityDbContext<User>
    {
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }
    }
}