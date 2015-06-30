using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class ActivityLogService
    {
        private readonly ActivityLogRepository repository;

        public ActivityLogService()
        {
            repository = new ActivityLogRepository();
        }

        public async Task<IEnumerable<ActivityLogModel>> GetActivityLogs()
        {
            return await repository.GetAll();
        }

        public async Task<ActivityLogModel> FindByid(int id)
        {
            return await repository.GetActivityLogById(id);
        }

        public async Task Create(ActivityLogCreateViewModel createModel, int userId)
        {
            ActivityLogModel model = new ActivityLogModel()
            {
                CreatedBy = userId,
                Title = createModel.Title,
                Title2 = createModel.Title2,
                Title3 = createModel.Title3,
                Organizers = createModel.Organizers,
                Location = createModel.Location,
                StartDate = createModel.StartDate,
                EndDate = createModel.EndDate,
                OnCampus = createModel.OnCampus,
                WebSite = createModel.WebSite,
                Notes = createModel.Notes
            };

            await repository.Create(model, userId);
        }
    }
}
