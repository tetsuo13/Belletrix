using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
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

        /// <summary>
        /// Manage the participants list for the selected activity.
        /// </summary>
        /// <param name="activityId">Current activity to associate with.</param>
        /// <param name="people">
        /// People in current session. Note that this collection may not have
        /// anything in it.
        /// </param>
        /// <returns>Nothing</returns>
        Task AssociatePeopleWithActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people);
    }
}
