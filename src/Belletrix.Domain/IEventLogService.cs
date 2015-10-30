﻿using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.Domain
{
    public interface IEventLogService
    {
        Task<IEnumerable<EventLogModel>> GetEvents();
        Task AddStudentEvent(EventLogModel model, int studentId, EventLogTypes eventType);
        Task AddStudentEvent(EventLogModel model, int modifiedBy, int studentId, EventLogTypes eventType);
    }
}
