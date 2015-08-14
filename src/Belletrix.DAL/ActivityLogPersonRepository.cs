using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class ActivityLogPersonRepository : IActivityLogPersonRepository
    {
        private readonly SqlConnection DbContext;
        private readonly IUnitOfWork UnitOfWork;

        public ActivityLogPersonRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            DbContext = unitOfWork.DbContext;
        }

        public async Task<int> CreatePerson(ActivityLogPersonModel model)
        {
            const string sql = @"
                INSERT INTO activity_log_person
                (
                    id, full_name, description,
                    phone, email
                )
                VALUES
                (
                    DEFAULT, @FullName, @Description,
                    @Phone, @Email
                )
                RETURNING id";

            int id;

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
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
                throw e;
            }

            return id;
        }

        public async Task AssociatePeopleWithActivity(int activityId, IEnumerable<ActivityLogParticipantModel> people)
        {
            const string sql = @"
                INSERT INTO activity_log_participant
                (event_id, person_id, participant_type)
                VALUES
                (@EventId, @PersonId, @Type)";

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
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
                throw e;
            }
        }

        private async Task<ActivityLogPersonModel> ReadPerson(SqlDataReader reader)
        {
            return new ActivityLogPersonModel()
            {
                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                FullName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FullName")),
                Description = await reader.GetText("Description"),
                PhoneNumber = await reader.GetText("Phone"),
                Email = await reader.GetText("Email")
            };
        }

        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            const string sql = @"
                SELECT      [Id], [FullName], [Description], [Phone], [Email]
                FROM        [ActivityLogPerson]
                ORDER BY    [FullName]";

            ICollection<ActivityLogPersonModel> people = new List<ActivityLogPersonModel>();

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
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
                throw e;
            }

            return people;
        }

        public async Task SaveChanges()
        {
            UnitOfWork.SaveChanges();
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
                FROM        [ActivityLogPerson]
                INNER JOIN  [ActivityLogParticipant] ON
                            [PersonId] = id
                WHERE       [EventId] = @ActivityId";

            ICollection<ActivityLogParticipantModel> people = new List<ActivityLogParticipantModel>();

            try
            {
                using (SqlCommand command = DbContext.CreateCommand())
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
                using (SqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@ActivityId", SqlDbType.Int).Value = activityId;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
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
                using (SqlCommand command = DbContext.CreateCommand())
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
                throw e;
            }
        }
    }
}
