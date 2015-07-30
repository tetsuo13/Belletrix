using Belletrix.Core;
using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.Domain
{
    public class ActivityLogPersonService : IActivityLogPersonService
    {
        private readonly IActivityLogPersonRepository Repository;

        public ActivityLogPersonService(IActivityLogPersonRepository repository)
        {
            Repository = repository;
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

            return await Repository.CreatePerson(model);
        }

        // TODO: Should this be only people who don't have a temporary session ID value?
        // A session value means this person is in the process of being added
        // to a new activity log.
        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            return await Repository.FindAllPeople();
        }

        public async Task SaveChanges()
        {
            await Repository.SaveChanges();
        }

        /// <summary>
        /// Find an existing person by their unique ID.
        /// </summary>
        /// <param name="id">Unique ID.</param>
        /// <returns>
        /// Person detail or <see langword="null"/> if no person is found by
        /// that ID.
        /// </returns>
        public async Task<ActivityLogPersonModel> FindPersonById(int id)
        {
            return await Repository.FindPersonById(id);
        }

        /// <summary>
        /// Finds all participants attached to a given activity log.
        /// </summary>
        /// <param name="activityId">Existing activity log ID.</param>
        /// <returns>All participants for the activity.</returns>
        public async Task<IEnumerable<ActivityLogParticipantModel>> FindActivityParticipants(int activityId)
        {
            return await Repository.FindActivityParticipants(activityId);
        }

        public void StartSession(HttpSessionStateBase session, Guid sessionId)
        {
            if (session[Constants.ActivityLogSessionName] == null)
            {
                session[Constants.ActivityLogSessionName] = new Dictionary<Guid, List<ActivityLogParticipantModel>>();
            }

            if (!(session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).ContainsKey(sessionId))
            {
                (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId] = new List<ActivityLogParticipantModel>();
            }
            else
            {
                // The session ID should always be unique, but just in case
                // the user revisits the activity log add/edit page using the
                // same ID more than once, reset everything.
                (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId].Clear();
            }
        }

        public void ClearSession(HttpSessionStateBase session, Guid sessionId)
        {
            (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).Remove(sessionId);
        }

        public async Task PopulateSession(HttpSessionStateBase session, Guid sessionId, int activityId)
        {
            IEnumerable<ActivityLogParticipantModel> participants = await Repository.FindActivityParticipants(activityId);
            (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId] = new List<ActivityLogParticipantModel>(participants);
        }

        public IEnumerable<ActivityLogParticipantModel> ParticipantsInSession(HttpSessionStateBase session,
            Guid sessionId)
        {
            if (session[Constants.ActivityLogSessionName] != null &&
                (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).ContainsKey(sessionId))
            {
                return (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId];
            }

            return null;
        }

        public void AddParticipantToSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant)
        {
            if (session[Constants.ActivityLogSessionName] == null)
            {
                session[Constants.ActivityLogSessionName] = new Dictionary<Guid, List<ActivityLogParticipantModel>>();
            }

            if (!(session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).ContainsKey(sessionId))
            {
                (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId] = new List<ActivityLogParticipantModel>();
            }

            (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId].Add(participant);
        }

        public void RemoveParticipantFromSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant)
        {
            List<ActivityLogParticipantModel> participants = (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId];

            int index = participants.FindIndex(x => x.Person.Id == participant.Person.Id);

            participants.RemoveAt(index);
        }

        public bool ContainsAssociatedPeople(HttpSessionStateBase session, Guid sessionId)
        {
            return session[Constants.ActivityLogSessionName] != null &&
                (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>).ContainsKey(sessionId) &&
                (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId].Count > 0;
        }
    }
}
