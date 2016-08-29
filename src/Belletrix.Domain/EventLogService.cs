using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public class EventLogService : IEventLogService
    {
        private readonly IEventLogRepository EventLogRepository;

        public EventLogService(IEventLogRepository eventLogRepository)
        {
            EventLogRepository = eventLogRepository;
        }

        public async Task<IEnumerable<EventLogModel>> GetEvents()
        {
            return await EventLogRepository.GetEvents();
        }

        public async Task AddStudentEvent(int studentId, EventLogTypes eventType, string remoteIp)
        {
            await EventLogRepository.AddStudentEvent(studentId, eventType, remoteIp);
        }

        public async Task AddStudentEvent(int modifiedBy, int studentId,
            EventLogTypes eventType, string remoteIp)
        {
            await EventLogRepository.AddStudentEvent(modifiedBy, studentId, eventType, remoteIp);
        }
    }
}
