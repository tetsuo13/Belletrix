using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IActivityLogPersonRepository
    {
        Task<int> CreatePerson(ActivityLogPersonModel model);
        Task AssociatePeopleWithActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people);
        Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople();
        void SaveChanges();
        Task<ActivityLogPersonModel> FindPersonById(int id);

        /// <summary>
        /// Finds all participants attached to a given activity log.
        /// </summary>
        /// <param name="activityId">Existing activity log ID.</param>
        /// <returns>All participants for the activity.</returns>
        Task<IEnumerable<ActivityLogParticipantModel>> FindActivityParticipants(int activityId);

        /// <summary>
        /// Remove all participants from the activity.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <returns>Nothing</returns>
        Task ClearParticipantsFromActivity(int activityId);

        /// <summary>
        /// Remove select participants from the activity.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <param name="people">People to remove.</param>
        /// <returns>Nothing</returns>
        Task ClearParticipantsFromActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people);
    }
}
