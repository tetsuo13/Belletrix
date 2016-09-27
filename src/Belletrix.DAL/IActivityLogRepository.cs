using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IActivityLogRepository
    {
        Task<ActivityLogModel> GetActivityById(int id);
        Task<IEnumerable<ActivityLogModel>> GetAllActivities();
        
        Task<int> InsertActivity(ActivityLogModel model, int userId);
        Task UpdateActivity(ActivityLogModel model);

        /// <summary>
        /// Associates a collection of activity log types with an activity
        /// log. This performs an "upsert," meaning it can be used both when
        /// creating a new activity log or updating an existing one.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <param name="types">One or more types to associate.</param>
        /// <returns>Nothing</returns>
        Task MergeActivityTypes(int activityId, IEnumerable<int> types);

        /// <summary>
        /// Get all types for a given activity log.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <returns>Types for the activity.</returns>
        Task<ActivityLogTypes[]> GetActivityTypes(int activityId);

        Task<bool> TransferOwnership(int oldId, int newId);
    }
}
