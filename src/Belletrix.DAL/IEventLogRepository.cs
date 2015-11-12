using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IEventLogRepository
    {
        Task<IEnumerable<EventLogModel>> GetEvents();
        Task AddStudentEvent(int studentId, EventLogTypes eventType);
        Task AddStudentEvent(int modifiedBy, int studentId, EventLogTypes eventType);
        void SaveChanges();
    }
}
