using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IActivityLogRepository
    {
        Task<ActivityLogModel> GetActivityById(int id);
        Task<IEnumerable<ActivityLogModel>> GetAllActivities();
        
        Task<int> InsertActivity(ActivityLogModel model, int userId);
        Task UpdateActivity(ActivityLogModel model);
        Task SaveChanges();
    }
}
