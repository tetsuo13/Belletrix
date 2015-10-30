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

        public async Task AddStudentEvent(EventLogModel model, int studentId, EventLogTypes eventType)
        {
            await EventLogRepository.AddStudentEvent(model, studentId, eventType);
        }

        public async Task AddStudentEvent(EventLogModel model, int modifiedBy, int studentId, EventLogTypes eventType)
        {
            await EventLogRepository.AddStudentEvent(model, modifiedBy, studentId, eventType);
        }
    }
}
