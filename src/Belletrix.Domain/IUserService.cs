using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IUserService
    {
        void UpdateLastLogin(string username);
        Task<IEnumerable<UserModel>> GetUsers(string username = null);
        Task<UserModel> GetUser(string username);
        Task<UserModel> GetUser(int id);
        void Update(UserModel model);
        void InsertUser(UserModel model);
    }
}
