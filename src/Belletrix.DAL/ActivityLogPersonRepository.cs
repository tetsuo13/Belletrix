using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Data;
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
                (@FullName, @Description, @Phone, @Email)";

            int id;

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@FullName", SqlDbType.VarChar, 128).Value = model.FullName;
                    command.Parameters.Add("@Description", SqlDbType.VarChar, 256).Value = !String.IsNullOrEmpty(model.Description) ? (object)model.Description : DBNull.Value;
                    command.Parameters.Add("@Phone", SqlDbType.VarChar, 32).Value = !String.IsNullOrEmpty(model.PhoneNumber) ? (object)model.PhoneNumber : DBNull.Value;
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = !String.IsNullOrEmpty(model.Email) ? (object)model.Email : DBNull.Value;

                    id = (int)await command.ExecuteScalarAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            return id;
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
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    SqlParameter eventIdParam = new SqlParameter("@EventId", SqlDbType.Int);
                    eventIdParam.Value = activityId;

                    SqlParameter personIdParam = new SqlParameter("@PersonId", SqlDbType.Int);
                    SqlParameter typeParam = new SqlParameter("@Type", SqlDbType.Int);

                    command.Parameters.Add(eventIdParam);
                    command.Parameters.Add(personIdParam);
                    command.Parameters.Add(typeParam);

                    command.Prepare();

                    foreach (ActivityLogParticipantModel person in people)
                    {
                        personIdParam.Value = person.Person.Id;
                        typeParam.Value = person.Type;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        private async Task<ActivityLogPersonModel> ReadPerson(SqlDataReader reader)
        {
            return new ActivityLogPersonModel()
            {
                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                FullName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FullName")),
                Description = await reader.GetValueOrDefault<string>("Description"),
                PhoneNumber = await reader.GetValueOrDefault<string>("Phone"),
                Email = await reader.GetValueOrDefault<string>("Email")
            };
        }

        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            const string sql = @"
                SELECT      [Id], [FullName], [Description], [Phone], [Email]
                FROM        [dbo].[ActivityLogPerson]
                ORDER BY    [FullName]";

            ICollection<ActivityLogPersonModel> people = new List<ActivityLogPersonModel>();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            people.Add(await ReadPerson(reader));
                        }
                    }
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
                SELECT      [Id], [FullName], [Description], [Phone], [Email], [ParticipantType]
                FROM        [dbo].[ActivityLogPerson]
                INNER JOIN  [dbo].[ActivityLogParticipant] ON
                            [PersonId] = id
                WHERE       [EventId] = @ActivityId";

            ICollection<ActivityLogParticipantModel> people = new List<ActivityLogParticipantModel>();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@ActivityId", SqlDbType.Int).Value = activityId;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            people.Add(new ActivityLogParticipantModel()
                            {
                                Event = new ActivityLogModel { Id = activityId },
                                Person = await ReadPerson(reader),
                                Type = await reader.GetFieldValueAsync<ActivityLogParticipantTypes>(reader.GetOrdinal("ParticipantType"))
                            });
                        }
                    }
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
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@ActivityId", SqlDbType.Int).Value = activityId;
                    await command.ExecuteNonQueryAsync();
                }
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
                            [PersonId] IN ({0})";

            string[] paramNames = people.Select((s, i) => "@param" + i.ToString()).ToArray();

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = String.Format(sql, String.Join(",", paramNames));

                    command.Parameters.Add("@ActivityId", SqlDbType.Int).Value = activityId;

                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        command.Parameters.AddWithValue(paramNames[i], people.ElementAt(i));
                    }

                    await command.ExecuteNonQueryAsync();
                }
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
