using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.DAL
{
    public class ActivityLogPersonRepository : IActivityLogPersonRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public ActivityLogPersonRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<int> CreatePerson(ActivityLogPersonModel model)
        {
            const string sql = @"
                INSERT INTO [dbo].[ActivityLogPerson]
                ([FullName], [Description], [Phone], [Email])
                OUTPUT INSERTED.Id
                VALUES
                (@FullName, @Description, @PhoneNumber, @Email)";

            try
            {
                return await UnitOfWork.Context().ExecuteAsync(sql, model);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task AssociatePeopleWithActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people)
        {
            const string sql = @"
                INSERT INTO [dbo].[ActivityLogParticipant]
                ([EventId], [PersonId], [ParticipantType])
                VALUES
                (@EventId, @PersonId, @Type)";

            try
            {
                foreach (ActivityLogParticipantModel person in people)
                {
                    await UnitOfWork.Context().ExecuteAsync(sql,
                        new
                        {
                            EventId = activityId,
                            PersonId = person.Person.Id,
                            Type = person.Type
                        });
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            const string sql = @"
                SELECT      [Id], [FullName], [Description], [Phone] AS PhoneNumber, [Email]
                FROM        [dbo].[ActivityLogPerson]
                ORDER BY    [FullName]";

            try
            {
                return await UnitOfWork.Context().QueryAsync<ActivityLogPersonModel>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task<ActivityLogPersonModel> FindPersonById(int id)
        {
            IEnumerable<ActivityLogPersonModel> people = await FindAllPeople();
            return people.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Finds all participants attached to a given activity log.
        /// </summary>
        /// <param name="activityId">Existing activity log ID.</param>
        /// <returns>All participants for the activity.</returns>
        public async Task<IEnumerable<ActivityLogParticipantModel>> FindActivityParticipants(int activityId)
        {
            const string sql = @"
                SELECT      [Id], [FullName], [Description], [Phone] AS PhoneNumber, [Email], [ParticipantType]
                FROM        [dbo].[ActivityLogPerson]
                INNER JOIN  [dbo].[ActivityLogParticipant] ON
                            [PersonId] = id
                WHERE       [EventId] = @ActivityId";

            ICollection<ActivityLogParticipantModel> people = new List<ActivityLogParticipantModel>();

            try
            {
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<ActivityLogParticipantModel>(sql,
                    new { ActivityId = activityId });

                foreach (IDictionary<string, object> row in rows)
                {
                    people.Add(new ActivityLogParticipantModel()
                    {
                        Event = new ActivityLogModel { Id = activityId },
                        Person = new ActivityLogPersonModel()
                        {
                            Id = (int)row["Id"],
                            FullName = (string)row["FullName"],
                            Description = row["Description"] as string,
                            PhoneNumber = row["PhoneNumber"] as string,
                            Email = row["Email"] as string
                        },
                        Type = (ActivityLogParticipantTypes)(int)row["ParticipantType"]
                    });
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            return people;
        }

        /// <summary>
        /// Remove all participants from the activity.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <returns>Nothing</returns>
        public async Task ClearParticipantsFromActivity(int activityId)
        {
            const string sql = @"
                DELETE FROM [dbo].[ActivityLogParticipant]
                WHERE       [EventId] = @ActivityId";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { ActivityId = activityId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        /// <summary>
        /// Remove select participants from the activity.
        /// </summary>
        /// <param name="activityId">Activity log ID.</param>
        /// <param name="people">People to remove.</param>
        /// <returns>Nothing</returns>
        public async Task ClearParticipantsFromActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people)
        {
            string sql = @"
                DELETE FROM [dbo].[ActivityLogParticipant]
                WHERE       [EventId] = @ActivityId AND
                            [PersonId] IN (@PersonIds)";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        ActivityId = activityId,
                        PersonIds = people.Select(x => x.Person.Id)
                    });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }
    }
}
