using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class ActivityLogService
    {
        private readonly ActivityLogRepository repository;

        public ActivityLogService()
        {
            repository = new ActivityLogRepository();
        }

        public async Task<IEnumerable<ActivityLogModel>> GetActivityLogs()
        {
            return await repository.GetAll();
        }

        public async Task<ActivityLogModel> FindByid(int id)
        {
            return await repository.GetActivityLogById(id);
        }

        public async Task Create(ActivityLogCreateViewModel createModel, int userId)
        {
            ActivityLogModel model = (ActivityLogModel)createModel;
            model.CreatedBy = userId;

            await repository.Create(model, userId);
        }

        public async Task<int> CreatePerson(ActivityLogPersonCreateViewModel createModel)
        {
            ActivityLogPersonModel model = new ActivityLogPersonModel()
            {
                FullName = createModel.FullName,
                Email = createModel.Email,
                Description = createModel.Description,
                PhoneNumber = createModel.PhoneNumber,
                SessionId = createModel.SessionId
            };

            return await repository.CreatePerson(model);
        }

        public async Task Save(ActivityLogEditViewModel saveModel)
        {
            ActivityLogModel model = (ActivityLogModel)saveModel;
            await repository.Save(model);
        }
    }
}
