using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IActivityLogService
    {
        Task<IEnumerable<ActivityLogModel>> GetActivityLogs();
        Task<ActivityLogModel> FindByid(int id);
        Task<int> InsertActivity(ActivityLogCreateViewModel createModel, int userId);
        Task UpdateActivity(ActivityLogEditViewModel saveModel);
        Task SaveChanges();
    }
}
