using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IActivityLogPersonRepository
    {
        Task<int> CreatePerson(ActivityLogPersonModel model);
        Task ClearSessionIdFromPeople(IEnumerable<ActivityLogPersonModel> people);
        Task AssociatePeopleWithActivity(int activityId, Guid sessionId,
            IEnumerable<ActivityLogParticipantModel> people);
        Task SaveChanges();
    }
}
