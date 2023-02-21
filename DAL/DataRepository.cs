using Data;
using Microsoft.AspNetCore.Identity;

namespace DAL
{
    public class DataRepository : IDataRepository
    {
        private readonly ContactsDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DataRepository(ContactsDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager) 
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        public Task ChangePassword()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUsernameInUse(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Login(string username, string password, bool rememberMe)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, false);
            return result.Succeeded;
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task Register()
        {
            throw new NotImplementedException();
        }
    }
}