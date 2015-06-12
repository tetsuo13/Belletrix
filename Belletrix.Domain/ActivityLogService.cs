using Belletrix.DAL;
using Belletrix.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public IEnumerable<ActivityLogListViewModel> GetActivityLogs()
        {
            return repository.GetActivityLogs();
        }
    }
}
