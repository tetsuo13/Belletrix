using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IActivityLogPersonRepository
    {
        Task<int> CreatePerson(ActivityLogPersonModel model);
        Task ClearSessionIdFromPeople(IEnumerable<ActivityLogPersonModel> people);
        Task AssociatePeopleWithActivity(int activityId, Guid sessionId,
            IEnumerable<ActivityLogParticipantModel> people);
        Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople();
        Task SaveChanges();
        Task<ActivityLogPersonModel> FindPersonById(int id);
    }
}
