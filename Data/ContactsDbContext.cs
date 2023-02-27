using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalkBack.Models;

namespace TalkBack.Data
{
    public class ContactsDbContext : IdentityDbContext<User>
    {
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }
    }
}