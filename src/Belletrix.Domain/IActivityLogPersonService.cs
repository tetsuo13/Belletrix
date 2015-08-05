using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.Domain
{
    public interface IActivityLogPersonService
    {
        Task<int> CreatePerson(ActivityLogPersonCreateViewModel createModel);
        Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople();
        Task SaveChanges();

        /// <summary>
        /// Find an existing person by their unique ID.
        /// </summary>
        /// <param name="id">Unique ID.</param>
        /// <returns>
        /// Person detail or <see langword="null"/> if no person is found by
        /// that ID.
        /// </returns>
        Task<ActivityLogPersonModel> FindPersonById(int id);

        /// <summary>
        /// Finds all participants attached to a given activity log.
        /// </summary>
        /// <param name="activityId">Existing activity log ID.</param>
        /// <returns>All participants for the activity.</returns>
        Task<IEnumerable<ActivityLogParticipantModel>> FindActivityParticipants(int activityId);

        void StartSession(HttpSessionStateBase session, Guid sessionId);
        Task PopulateSession(HttpSessionStateBase session, Guid sessionId, int activityId);
        IEnumerable<ActivityLogParticipantModel> ParticipantsInSession(HttpSessionStateBase session, Guid sessionId);

        void AddParticipantToSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant);

        void RemoveParticipantFromSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant);

        void ClearSession(HttpSessionStateBase session, Guid sessionId);
    }
}
