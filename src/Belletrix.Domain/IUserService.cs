using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IUserService
    {
        Task UpdateLastLogin(string username);
        Task<IEnumerable<UserModel>> GetUsers(string username = null);
        Task<UserModel> GetUser(string username);
        Task<UserModel> GetUser(int id);
        Task Update(UserModel model);
        Task InsertUser(UserModel model);
        Task<GenericResult> Delete(int id, int currentUserId);
    }
}
