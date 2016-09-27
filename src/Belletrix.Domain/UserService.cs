using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Belletrix.Domain
{
    public class UserService : IUserService
    {
        private readonly IActivityLogRepository ActivityLogRepository;
        private readonly IEventLogRepository EventLogRepository;
        private readonly IPromoRepository PromoRepository;
        private readonly IUserRepository UserRepository;
        private readonly IStudentNoteRepository StudentNoteRepository;

        public UserService(IActivityLogRepository activityLogRepository, IEventLogRepository eventLogRepository,
            IPromoRepository promoRepository, IUserRepository userRepository,
            IStudentNoteRepository studentNoteRepository)
        {
            ActivityLogRepository = activityLogRepository;
            EventLogRepository = eventLogRepository;
            PromoRepository = promoRepository;
            UserRepository = userRepository;
            StudentNoteRepository = studentNoteRepository;
        }

        public async Task UpdateLastLogin(string username)
        {
            await UserRepository.UpdateLastLogin(username);
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

        public async Task Update(UserModel model)
        {
            await UserRepository.Update(model);
        }

        public async Task InsertUser(UserModel model)
        {
            await UserRepository.InsertUser(model);
        }

        public async Task<GenericResult> Delete(int id, int currentUserId)
        {
            GenericResult result = new GenericResult();

            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (!await EventLogRepository.TransferOwnership(id, currentUserId))
                    {
                        result.Message = "Error transfering event logs";
                    }
                    else if (!await ActivityLogRepository.TransferOwnership(id, currentUserId))
                    {
                        result.Message = "Error transfering activity logs";
                    }
                    else if (!await PromoRepository.TransferOwnership(id, currentUserId))
                    {
                        result.Message = "Error transfering activity logs";
                    }
                    else if (!await StudentNoteRepository.TransferOwnership(id, currentUserId))
                    {
                        result.Message = "Error transfering student notes";
                    }
                    else if (!await UserRepository.Delete(id))
                    {
                        result.Message = "Error deleting user";
                    }
                    else
                    {
                        result.Result = true;
                        scope.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                result.Result = false;
                result.Message = "Error completing transaction scope: " + e.Message;
            }

            return result;
        }
    }
}
