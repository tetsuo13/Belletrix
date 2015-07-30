using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    /// <summary>
    /// Activity Log business logic. Generically named since the class needs
    /// to encompass multiple repositories related to the Activity Log.
    /// </summary>
    public interface IActivityService
    {
        Task<IEnumerable<ActivityLogModel>> GetActivityLogs();
        Task<ActivityLogModel> FindByid(int id);
        Task<int> InsertActivity(ActivityLogCreateViewModel createModel, int userId);
        Task UpdateActivity(ActivityLogEditViewModel saveModel);
        Task SaveChanges();

        Task AssociatePeopleWithActivity(int activityId, Guid sessionId, IEnumerable<ActivityLogParticipantModel> people);
    }
}
