using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Belletrix.Entity.Enum;

namespace Belletrix.Domain
{
    public class ActivityLogService : IActivityLogService
    {
        /// <summary>
        /// Session key associated with Activity Log. Used to store active
        /// participants while creating/editing an activity.
        /// </summary>
        public const string SessionName = "ActivityLog";

        private IActivityLogRepository repository;

        public ActivityLogService(IActivityLogRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<ActivityLogModel>> GetActivityLogs()
        {
            return await repository.GetAllActivities();
        }

        public async Task<ActivityLogModel> FindByid(int id)
        {
            return await repository.GetActivityById(id);
        }

        public async Task<int> InsertActivity(ActivityLogCreateViewModel createModel, int userId)
        {
            ActivityLogModel model = (ActivityLogModel)createModel;
            model.CreatedBy = userId;

            return await repository.InsertActivity(model, userId);
        }

        public async Task SaveChanges()
        {
            await repository.SaveChanges();
        }

        public async Task UpdateActivity(ActivityLogEditViewModel saveModel)
        {
            ActivityLogModel model = (ActivityLogModel)saveModel;
            await repository.UpdateActivity(model);
        }
    }
}
