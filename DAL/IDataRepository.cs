using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IDataRepository
    {
        public Task<bool> Login(string username, string password, bool rememberMe);
        public Task Logout();
        public Task Register();
        public Task<bool> IsUsernameInUse(string username);
        public Task ChangePassword();
    }
}
