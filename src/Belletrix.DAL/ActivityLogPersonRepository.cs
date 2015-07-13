using Belletrix.Entity.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
                    phone, email, session_id
                )
                VALUES
                (
                    DEFAULT, @FullName, @Description,
                    @Phone, @Email, @SessionId
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
                    command.Parameters.Add("@SessionId", NpgsqlTypes.NpgsqlDbType.Uuid).Value = model.SessionId;

                    id = (int)await command.ExecuteScalarAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }

            return id;
        }

        public async Task AssociatePeopleWithActivity(int activityId, Guid sessionId,
            IEnumerable<ActivityLogParticipantModel> people)
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
                e.Data["SQL"] = e;
                throw e;
            }
        }

        public async Task ClearSessionIdFromPeople(IEnumerable<ActivityLogPersonModel> people)
        {
            const string sql = @"
                UPDATE  activity_log_person
                SET     session_id = NULL
                WHERE   id IN (@PeopleIds)";

            try
            {
                using (NpgsqlCommand command = DbContext.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Add("@PeopleIds", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Numeric).Value = people.Select(x => x.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = e;
                throw e;
            }
        }

        public async Task SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
