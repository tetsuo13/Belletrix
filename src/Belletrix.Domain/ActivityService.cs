using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Belletrix.Domain
{
    /// <summary>
    /// Activity Log business logic. Generically named since the class needs
    /// to encompass multiple repositories related to the Activity Log.
    /// </summary>
    public class ActivityService : IActivityService
    {
        /// <summary>
        /// Session key associated with Activity Log. Used to store active
        /// participants while creating/editing an activity.
        /// </summary>
        private const string ParticipantsSessionName = "ActivityLog";

        private readonly IActivityLogRepository ActivityLogRepository;
        private readonly IActivityLogPersonRepository ActivityLogPersonRepository;
        private readonly IUserRepository UserRepository;

        public ActivityService(IActivityLogRepository activityLogRepository,
            IActivityLogPersonRepository activityLogPersonRepository, IUserRepository userRepository)
        {
            ActivityLogRepository = activityLogRepository;
            ActivityLogPersonRepository = activityLogPersonRepository;
            UserRepository = userRepository;
        }

        public async Task<IEnumerable<ActivityLogModel>> GetActivityLogs()
        {
            return await ActivityLogRepository.GetAllActivities();
        }

        public async Task<ActivityLogModel> FindById(int id)
        {
            return await ActivityLogRepository.GetActivityById(id);
        }

        public async Task<ActivityLogViewViewModel> FindAllInfoById(int id)
        {
            ActivityLogModel activityLogModel = await FindById(id);

            if (activityLogModel == null)
            {
                return null;
            }

            ActivityLogViewViewModel viewModel = new ActivityLogViewViewModel();
            viewModel.ActivityLog = activityLogModel;
            viewModel.Participants = await ActivityLogPersonRepository.FindActivityParticipants(id);
            viewModel.CreatedBy = await UserRepository.GetUser(activityLogModel.CreatedBy);

            return viewModel;
        }

        public async Task<int> InsertActivity(ActivityLogCreateViewModel createModel, int userId)
        {
            ActivityLogModel model = (ActivityLogModel)createModel;
            model.CreatedBy = userId;

            int activityId = await ActivityLogRepository.InsertActivity(model, userId);
            await ActivityLogRepository.MergeActivityTypes(activityId, createModel.Types);

            return activityId;
        }

        public async Task UpdateActivity(ActivityLogEditViewModel saveModel)
        {
            ActivityLogModel model = (ActivityLogModel)saveModel;
            await ActivityLogRepository.UpdateActivity(model);
            await ActivityLogRepository.MergeActivityTypes(model.Id, model.Types.Cast<int>());
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
        public async Task AssociatePeopleWithActivity(HttpSessionStateBase session, int activityId, Guid sessionId)
        {
            List<ActivityLogParticipantModel> start = (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].StartingList;
            List<ActivityLogParticipantModel> end = (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].WorkingList;

            // Started and finished with no participants.
            if (start.Count == 0 && end.Count == 0)
            {
                return;
            }

            // Started with no participants, ending with at least one. All new
            // for this activity log.
            if (start.Count == 0 && end.Count > 0)
            {
                await ActivityLogPersonRepository.AssociatePeopleWithActivity(activityId, end);
            }
            else if (start.Count > 0 && end.Count == 0)
            {
                // User cleared all participants from the activity log.

                await ActivityLogPersonRepository.ClearParticipantsFromActivity(activityId);
            }
            else
            {
                // Last use case: number of participants at the end doesn't
                // match what was started with. Participants were added or
                // removed from an existing activity log.

                IEnumerable<ActivityLogParticipantModel> removed = start.Except(end);
                IEnumerable<ActivityLogParticipantModel> added = end.Except(start);

                if (removed.Any())
                {
                    await ActivityLogPersonRepository.ClearParticipantsFromActivity(activityId, removed);
                }

                if (added.Any())
                {
                    await ActivityLogPersonRepository.AssociatePeopleWithActivity(activityId, added);
                }
            }
        }

        public async Task PopulateSession(HttpSessionStateBase session, Guid sessionId, int activityId)
        {
            IEnumerable<ActivityLogParticipantModel> participants = await ActivityLogPersonRepository.FindActivityParticipants(activityId);
            (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].StartingList = new List<ActivityLogParticipantModel>(participants);
            (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].WorkingList = new List<ActivityLogParticipantModel>(participants);
        }

        public void AddParticipantToSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant)
        {
            (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].WorkingList.Add(participant);
        }

        public IEnumerable<ActivityLogParticipantModel> ParticipantsInSession(HttpSessionStateBase session,
            Guid sessionId)
        {
            if (session[ParticipantsSessionName] != null &&
                (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>).ContainsKey(sessionId))
            {
                return (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].WorkingList;
            }

            return null;
        }

        public void StartSession(HttpSessionStateBase session, Guid sessionId)
        {
            if (session[ParticipantsSessionName] == null)
            {
                session[ParticipantsSessionName] = new Dictionary<Guid, ActivityLogSessionModel>();
            }

            if (!(session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>).ContainsKey(sessionId))
            {
                ActivityLogSessionModel model = new ActivityLogSessionModel()
                {
                    StartingList = new List<ActivityLogParticipantModel>(),
                    WorkingList = new List<ActivityLogParticipantModel>()
                };

                (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>).Add(sessionId, model);
            }
            else
            {
                // The session ID should always be unique, but just in case
                // the user revisits the activity log add/edit page using the
                // same ID more than once, reset everything.
                (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].StartingList.Clear();
                (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].WorkingList.Clear();
            }
        }

        public void ClearSession(HttpSessionStateBase session, Guid sessionId)
        {
            (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>).Remove(sessionId);
        }

        public void RemoveParticipantFromSession(HttpSessionStateBase session, Guid sessionId,
            ActivityLogParticipantModel participant)
        {
            List<ActivityLogParticipantModel> participants = (session[ParticipantsSessionName] as Dictionary<Guid, ActivityLogSessionModel>)[sessionId].WorkingList;

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

        /// <summary>
        /// A dictionary of
        /// <see cref="Belletrix.Entity.Enum.ActivityLogTypes"/> values as
        /// keys and associated Bootstrap label class names.
        /// </summary>
        /// <returns>Enum values and Bootstrap class names.</returns>
        public Dictionary<int, string> GetActivityTypeLabels()
        {
            return new Dictionary<int, string>()
            {
                { 1, "label-default" },
                { 2, "label-primary" },
                { 3, "label-success" },
                { 4, "label-info" },
                { 5, "label-warning" },
                { 6, "label-danger" },
                { 7, "label-default" },
                { 8, "label-primary" },
                { 9, "label-success" }
            };
        }

        public async Task InsertActivityBlock(HttpSessionStateBase session, ActivityLogCreateViewModel model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                int activityId = await InsertActivity(model, (session["User"] as UserModel).Id);
                await AssociatePeopleWithActivity(session, activityId, model.SessionId);
                ClearSession(session, model.SessionId);

                scope.Complete();
            }
        }

        public async Task UpdateActivityBlock(HttpSessionStateBase session, ActivityLogEditViewModel model)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                await UpdateActivity(model);
                await AssociatePeopleWithActivity(session, model.Id, model.SessionId);
                ClearSession(session, model.SessionId);

                scope.Complete();
            }
        }
    }
}
