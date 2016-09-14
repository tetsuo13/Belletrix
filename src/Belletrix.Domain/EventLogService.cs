using Belletrix.DAL;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System;
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

        public async Task<IEnumerable<EventLogViewModel>> GetEvents()
        {
            return await EventLogRepository.GetEvents();
        }

        public async Task AddStudentEvent(int studentId, EventLogTypes eventType, string remoteIp)
        {
            EventLogModel log = new EventLogModel()
            {
                Date = DateTime.Now,
                StudentId = studentId,
                Type = (int)eventType,
                IPAddress = remoteIp
            };

            await AddStudentEvent(log);
        }

        public async Task AddStudentEvent(int modifiedBy, int studentId,
            EventLogTypes eventType, string remoteIp)
        {
            EventLogModel log = new EventLogModel()
            {
                Date = DateTime.Now,
                ModifiedBy = modifiedBy,
                StudentId = studentId,
                Type = (int)eventType,
                IPAddress = remoteIp
            };

            await AddStudentEvent(log);
        }

        private async Task AddStudentEvent(EventLogModel log)
        {
            await EventLogRepository.AddStudentEvent(log);
        }
    }
}
