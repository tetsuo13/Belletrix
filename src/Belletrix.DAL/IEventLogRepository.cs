﻿using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IEventLogRepository
    {
        Task<IEnumerable<EventLogViewModel>> GetEvents(int numEvents);
        Task AddStudentEvent(EventLogModel log);
        Task<bool> TransferOwnership(int oldId, int newId);
        Task<bool> DeleteStudent(int id);
    }
}
