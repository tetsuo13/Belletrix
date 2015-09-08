using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public EventLogRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EventLogModel>> GetEvents()
        {
            ICollection<EventLogModel> events = new List<EventLogModel>();

            const string sql = @"
                SELECT          TOP(8) e.Id, e.Date, e.ModifiedBy,
                                e.StudentId, e.UserId, e.Type,
                                e.Action, u.FirstName, u.LastName,
                                s.FirstName AS StudentFirstName,
                                s.LastName AS StudentLastName,
                                us.FirstName AS UserFirstName,
                                us.LastName AS UserLastName
                FROM            [dbo].[EventLog] e
                INNER JOIN      [dbo].[Users] u ON
                                [ModifiedBy] = u.id
                LEFT OUTER JOIN [dbo].[Students] s ON
                                e.StudentId = s.id
                LEFT OUTER JOIN [dbo].[Users] us ON
                                e.UserId = us.id
                ORDER BY        [Date] DESC";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            StudentModel student = null;
                            int ord = reader.GetOrdinal("StudentId");
                            if (!reader.IsDBNull(ord))
                            {
                                student = new StudentModel()
                                {
                                    Id = await reader.GetFieldValueAsync<int>(ord),
                                    FirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("StudentFirstName")),
                                    LastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("StudentLastName"))
                                };
                            }

                            EventLogModel eventLog = new EventLogModel()
                            {
                                Id = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Id")),
                                EventDate = DateTimeFilter.UtcToLocal(await reader.GetFieldValueAsync<DateTime>(reader.GetOrdinal("Date"))),
                                ModifiedById = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("ModifiedBy")),
                                ModifiedByFirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("FirstName")),
                                ModifiedByLastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("LastName")),
                                Student = student,
                                Type = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("Type")),
                                Action = await reader.GetValueOrDefault<string>("Action")
                            };

                            ord = reader.GetOrdinal("UserId");

                            if (!reader.IsDBNull(ord))
                            {
                                eventLog.UserId = await reader.GetFieldValueAsync<int>(ord);
                                eventLog.UserFirstName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("UserFirstName"));
                                eventLog.UserLastName = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("UserLastName"));
                            }

                            eventLog.RelativeDate = DateTimeFilter.CalculateRelativeDate(eventLog.EventDate.ToUniversalTime());

                            events.Add(eventLog);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return events;
        }

        public async Task AddStudentEvent(EventLogModel model, int studentId, EventLogTypes eventType)
        {
            await AddStudentEvent(model, 0, studentId, eventType);
        }

        public async Task AddStudentEvent(EventLogModel model, int modifiedBy, int studentId, EventLogTypes eventType)
        {
            const string sql = @"
                INSERT INTO [dbo].[EventLog]
                ([Date], [ModifiedBy], [StudentId], [Type])
                VALUES
                (@Date, @ModifiedBy, @StudentId, @Type)";

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                    command.Parameters.Add("@Type", SqlDbType.Int).Value = (int)eventType;
                    command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

                    if (modifiedBy == 0)
                    {
                        command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = modifiedBy;
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

        public void SaveChanges()
        {
            UnitOfWork.SaveChanges();
        }
    }
}
