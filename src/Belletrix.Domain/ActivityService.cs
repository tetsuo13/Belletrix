using Belletrix.DAL;
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
    }
}
