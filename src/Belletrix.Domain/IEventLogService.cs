using Belletrix.Entity.Enum;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IEventLogService
    {
        Task<IEnumerable<EventLogViewModel>> GetEvents(int numEvents);
        Task AddStudentEvent(int studentId, EventLogTypes eventType, string remoteIp);
        Task AddStudentEvent(int modifiedBy, int studentId, EventLogTypes eventType, string remoteIp);
    }
}
