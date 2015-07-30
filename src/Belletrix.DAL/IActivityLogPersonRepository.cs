﻿using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public interface IActivityLogPersonRepository
    {
        Task<int> CreatePerson(ActivityLogPersonModel model);
        Task AssociatePeopleWithActivity(int activityId, Guid sessionId,
            IEnumerable<ActivityLogParticipantModel> people);
        Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople();
        Task SaveChanges();
        Task<ActivityLogPersonModel> FindPersonById(int id);

        /// <summary>
        /// Finds all participants attached to a given activity log.
        /// </summary>
        /// <param name="activityId">Existing activity log ID.</param>
        /// <returns>All participants for the activity.</returns>
        Task<IEnumerable<ActivityLogParticipantModel>> FindActivityParticipants(int activityId);
    }
}
