using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class ContactsDbContext : IdentityDbContext<User>
    {
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }
    }
}