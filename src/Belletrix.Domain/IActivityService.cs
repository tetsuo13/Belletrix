using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

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

        Task PopulateSession(HttpSessionStateBase session, Guid sessionId, int activityId);
        IEnumerable<ActivityLogParticipantModel> ParticipantsInSession(HttpSessionStateBase session, Guid sessionId);

        void RemoveParticipantFromSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant);

        void AddParticipantToSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant);

        void StartSession(HttpSessionStateBase session, Guid sessionId);
        void ClearSession(HttpSessionStateBase session, Guid sessionId);

        Task<int> CreatePerson(ActivityLogPersonCreateViewModel createModel);
        Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople();

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
