using Belletrix.Core;
using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.Domain
{
    /// <summary>
    /// Activity Log business logic. Generically named since the class needs
    /// to encompass multiple repositories related to the Activity Log.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IActivityLogRepository ActivityLogRepository;
        private readonly IActivityLogPersonRepository ActivityLogPersonRepository;

        public ActivityService(IActivityLogRepository activityLogRepository,
            IActivityLogPersonRepository activityLogPersonRepository)
        {
            ActivityLogRepository = activityLogRepository;
            ActivityLogPersonRepository = activityLogPersonRepository;
        }

        public async Task<IEnumerable<ActivityLogModel>> GetActivityLogs()
        {
            return await ActivityLogRepository.GetAllActivities();
        }

        public async Task<ActivityLogModel> FindByid(int id)
        {
            return await ActivityLogRepository.GetActivityById(id);
        }

        public async Task<int> InsertActivity(ActivityLogCreateViewModel createModel, int userId)
        {
            ActivityLogModel model = (ActivityLogModel)createModel;
            model.CreatedBy = userId;

            return await ActivityLogRepository.InsertActivity(model, userId);
        }

        public async Task UpdateActivity(ActivityLogEditViewModel saveModel)
        {
            ActivityLogModel model = (ActivityLogModel)saveModel;
            await ActivityLogRepository.UpdateActivity(model);
        }

        public async Task SaveChanges()
        {
            await ActivityLogRepository.SaveChanges();
            await ActivityLogPersonRepository.SaveChanges();
        }

        /// <summary>
        /// Manage the participants list for the selected activity.
        /// </summary>
        /// <param name="activityId">Current activity to associate with.</param>
        /// <param name="people">
        /// People in current session. Note that this collection may not have
        /// anything in it.
        /// </param>
        /// <returns>Nothing</returns>
        public async Task AssociatePeopleWithActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people)
        {
            if (people == null || !people.Any())
            {
                await ActivityLogPersonRepository.ClearParticipantsFromActivity(activityId);
            }
            else
            {
                await ActivityLogPersonRepository.AssociatePeopleWithActivity(activityId, people);
            }
        }

        public async Task PopulateSession(HttpSessionStateBase session, Guid sessionId, int activityId)
        {
            IEnumerable<ActivityLogParticipantModel> participants = await ActivityLogPersonRepository.FindActivityParticipants(activityId);
            (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId] = new List<ActivityLogParticipantModel>(participants);
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

        public void RemoveParticipantFromSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant)
        {
            List<ActivityLogParticipantModel> participants = (session[Constants.ActivityLogSessionName] as Dictionary<Guid, List<ActivityLogParticipantModel>>)[sessionId];

            int index = participants.FindIndex(x => x.Person.Id == participant.Person.Id);

            participants.RemoveAt(index);
        }

        // TODO: Should this be only people who don't have a temporary session ID value?
        // A session value means this person is in the process of being added
        // to a new activity log.
        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            return await ActivityLogPersonRepository.FindAllPeople();
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
            return await ActivityLogPersonRepository.FindPersonById(id);
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

            return await ActivityLogPersonRepository.CreatePerson(model);
        }
    }
}
