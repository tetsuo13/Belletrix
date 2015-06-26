using Belletrix.DAL;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;

namespace Belletrix.Domain
{
    public class ActivityLogService
    {
        private readonly ActivityLogRepository repository;

        public ActivityLogService()
        {
            repository = new ActivityLogRepository();
        }

        public IEnumerable<ActivityLogModel> GetActivityLogs()
        {
            return repository.GetAll();
        }

        public ActivityLogModel FindByid(int id)
        {
            return repository.GetActivityLogById(id);
        }

        public void Create(ActivityLogCreateViewModel createModel, int userId)
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

            repository.Create(model, userId);
        }
    }
}
