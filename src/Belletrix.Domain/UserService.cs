using Belletrix.DAL;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class UserService : IUserService
    {
        private IUserRepository UserRepository;

        public UserService(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public void UpdateLastLogin(string username)
        {
            UserRepository.UpdateLastLogin(username);
        }

        public async Task<IEnumerable<UserModel>> GetUsers(string username = null)
        {
            return await UserRepository.GetUsers(username);
        }

        public async Task<UserModel> GetUser(string username)
        {
            return await UserRepository.GetUser(username);
        }

        public async Task<UserModel> GetUser(int id)
        {
            return await UserRepository.GetUser(id);
        }

        public void UpdateUser(UserModel model, bool isAdmin)
        {
            UserRepository.UpdateUser(model, isAdmin);
            UserRepository.SaveChanges();
        }

        public void InsertUser(UserModel model)
        {
            UserRepository.InsertUser(model);
            UserRepository.SaveChanges();
        }
    }
}
