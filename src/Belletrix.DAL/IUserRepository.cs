using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IUserRepository
    {
        void UpdateLastLogin(string username);
        Task<IEnumerable<UserModel>> GetUsers(string username = null);
        Task<UserModel> GetUser(string username);
        Task<UserModel> GetUser(int id);
        void UpdateUser(UserModel model, bool isAdmin);
        void InsertUser(UserModel model);
    }
}
