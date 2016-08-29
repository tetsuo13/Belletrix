using Belletrix.Core;
using Belletrix.Entity.Enum;
using Belletrix.Entity.Model;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

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
                                us.LastName AS UserLastName,
                                IPAddress
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
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql);

                foreach (IDictionary<string, object> row in rows)
                {
                    StudentModel student = null;

                    // TODO: Check for key or does key always exist and value is null?
                    if (row.ContainsKey("StudentId") && row["StudentId"] != null)
                    {
                        student = new StudentModel()
                        {
                            Id = (int)row["StudentId"],
                            FirstName = (string)row["StudentFirstName"],
                            LastName = (string)row["StudentLastName"]
                        };
                    }

                    EventLogModel eventLog = new EventLogModel()
                    {
                        Id = (int)row["Id"],
                        EventDate = DateTimeFilter.UtcToLocal((DateTime)row["Date"]),
                        ModifiedById = (int)row["ModifiedBy"],
                        ModifiedByFirstName = (string)row["FirstName"],
                        ModifiedByLastName = (string)row["LastName"],
                        Student = student,
                        Type = (int)row["Type"],
                        Action = (string)row["Action"],
                        IPAddress = row["IPAddress"] as string
                    };

                    if (row.ContainsKey("UserId") && row["UserId"] != null)
                    {
                        eventLog.UserId = (int)row["UserId"];
                        eventLog.UserFirstName = (string)row["UserFirstName"];
                        eventLog.UserLastName = (string)row["UserLastName"];
                    }

                    eventLog.RelativeDate = DateTimeFilter.CalculateRelativeDate(eventLog.EventDate.ToUniversalTime());

                    events.Add(eventLog);
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }

            return events;
        }

        public async Task AddStudentEvent(int studentId, EventLogTypes eventType, string remoteIp)
        {
            await AddStudentEvent(0, studentId, eventType, remoteIp);
        }

        public async Task AddStudentEvent(int modifiedBy, int studentId,
            EventLogTypes eventType, string remoteIp)
        {
            const string sql = @"
                INSERT INTO [dbo].[EventLog]
                ([Date], [ModifiedBy], [StudentId], [Type], [IPAddress])
                VALUES
                (@Date, @ModifiedBy, @StudentId, @Type, @IpAddress)";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql,
                    new
                    {
                        Date = DateTime.Now.ToUniversalTime(),
                        ModifiedBy = modifiedBy == 0 ? null : (object)modifiedBy,
                        StudentId = studentId,
                        Type = (int)eventType,
                        IpAddress = remoteIp
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
