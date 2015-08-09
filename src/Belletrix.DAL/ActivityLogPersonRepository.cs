using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class ActivityLogPersonRepository : IActivityLogPersonRepository
    {
        private readonly NpgsqlConnection DbContext;
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
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@FullName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = model.FullName;
                    command.Parameters.Add("@Description", NpgsqlTypes.NpgsqlDbType.Varchar, 256).Value = !String.IsNullOrEmpty(model.Description) ? (object)model.Description : DBNull.Value;
                    command.Parameters.Add("@Phone", NpgsqlTypes.NpgsqlDbType.Varchar, 32).Value = !String.IsNullOrEmpty(model.PhoneNumber) ? (object)model.PhoneNumber : DBNull.Value;
                    command.Parameters.Add("@Email", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = !String.IsNullOrEmpty(model.Email) ? (object)model.Email : DBNull.Value;

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
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;

                    NpgsqlParameter eventIdParam = new NpgsqlParameter("@EventId", DbType.Int32);
                    eventIdParam.Value = activityId;

                    NpgsqlParameter personIdParam = new NpgsqlParameter("@PersonId", DbType.Int32);
                    NpgsqlParameter typeParam = new NpgsqlParameter("@Type", DbType.Int32);

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

        private async Task<ActivityLogPersonModel> ReadPerson(DbDataReader reader)
        {
            return new ActivityLogPersonModel()
            {
                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("id")),
                FullName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("full_name")),
                Description = await reader.GetText("description"),
                PhoneNumber = await reader.GetText("phone"),
                Email = await reader.GetText("email")
            };
        }

        public async Task<IEnumerable<ActivityLogPersonModel>> FindAllPeople()
        {
            const string sql = @"
                SELECT      id, full_name, description, phone, email
                FROM        activity_log_person
                ORDER BY    full_name";

            ICollection<ActivityLogPersonModel> people = new List<ActivityLogPersonModel>();

            try
            {
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
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
                SELECT      id, full_name, description, phone, email, participant_type
                FROM        activity_log_person person
                INNER JOIN  activity_log_participant participant ON
                            person_id = id
                WHERE       event_id = @ActivityId";

            ICollection<ActivityLogParticipantModel> people = new List<ActivityLogParticipantModel>();

            try
            {
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@ActivityId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = activityId;

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            people.Add(new ActivityLogParticipantModel()
                            {
                                Event = new ActivityLogModel { Id = activityId },
                                Person = await ReadPerson(reader),
                                Type = await reader.GetFieldValueAsync<ActivityLogParticipantTypes>(reader.GetOrdinal("participant_type"))
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
                DELETE FROM activity_log_participant
                WHERE       event_id = @ActivityId";

            try
            {
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@ActivityId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = activityId;
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
            const string sql = @"
                DELETE FROM activity_log_participant
                WHERE       event_id = @ActivityId AND
                            person_id = ANY(@PeopleIds)";

            try
            {
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@ActivityId", NpgsqlTypes.NpgsqlDbType.Numeric).Value = activityId;
                    command.Parameters.Add("@PeopleIds", NpgsqlTypes.NpgsqlDbType.Numeric | NpgsqlTypes.NpgsqlDbType.Array).Value = people.Select(x => x.Person.Id);

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
