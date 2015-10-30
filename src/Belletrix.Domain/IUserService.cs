using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IUserService
    {
        void UpdateLastLogin(string username);
        Task<IEnumerable<UserModel>> GetUsers(string username = null);
        Task<UserModel> GetUser(string username);
        Task<UserModel> GetUser(int id);
        void UpdateUser(UserModel model, bool isAdmin);
        void InsertUser(UserModel model);
    }
}
