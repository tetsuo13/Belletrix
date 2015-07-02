using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Linq;
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
            model.Types = createModel.Types.Cast<ActivityLogTypes>().ToArray();

            await repository.Create(model, userId);
        }

        public async Task Save(ActivityLogEditViewModel saveModel)
        {
            ActivityLogModel model = (ActivityLogModel)saveModel;
            await repository.Save(model);
        }
    }
}
