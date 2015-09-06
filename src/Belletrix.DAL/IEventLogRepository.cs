using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IEventLogRepository
    {
        Task<IEnumerable<EventLogModel>> GetEvents();
        Task AddStudentEvent(EventLogModel model, SqlTransaction transaction, int studentId, EventLogTypes eventType);
        Task AddStudentEvent(EventLogModel model, SqlTransaction transaction, int modifiedBy, int studentId,
            EventLogTypes eventType);
    }
}
