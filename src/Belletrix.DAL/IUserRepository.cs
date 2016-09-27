using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IUserRepository
    {
        Task UpdateLastLogin(string username);
        Task<IEnumerable<UserModel>> GetUsers(string username = null);
        Task<UserModel> GetUser(string username);
        Task<UserModel> GetUser(int id);
        Task Update(UserModel model);
        Task InsertUser(UserModel model);
        Task<bool> Delete(int id);
    }
}
