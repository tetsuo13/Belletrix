using Belletrix.Core;
using Belletrix.Entity.Model;
using Belletrix.Entity.ViewModel;
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

        public async Task<IEnumerable<EventLogViewModel>> GetEvents(int numEvents)
        {
            ICollection<EventLogViewModel> events = new List<EventLogViewModel>();

            const string sql = @"
                SELECT          TOP(@NumEvents) e.Id, e.Date, e.ModifiedBy,
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
                IEnumerable<dynamic> rows = await UnitOfWork.Context().QueryAsync<dynamic>(sql,
                    new { NumEvents = numEvents });

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

                    EventLogViewModel eventLog = new EventLogViewModel()
                    {
                        Id = (int)row["Id"],
                        EventDate = DateTimeFilter.UtcToLocal((DateTime)row["Date"]),
                        ModifiedById = (int)row["ModifiedBy"],
                        ModifiedByFirstName = (string)row["FirstName"],
                        ModifiedByLastName = (string)row["LastName"],
                        Student = student,
                        Type = (int)row["Type"],
                        Action = (string)row["Action"]
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

        public async Task AddStudentEvent(EventLogModel log)
        {
            const string sql = @"
                INSERT INTO [dbo].[EventLog]
                ([Date], [ModifiedBy], [StudentId], [Type], [IPAddress])
                VALUES
                (@Date, @ModifiedBy, @StudentId, @Type, @IpAddress)";

            try
            {
                log.Date = log.Date.ToUniversalTime();
                await UnitOfWork.Context().ExecuteAsync(sql, log);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                throw e;
            }
        }

        public async Task<bool> TransferOwnership(int oldId, int newId)
        {
            const string sql = @"
                UPDATE  [dbo].[EventLog]
                SET     [ModifiedBy] = CASE WHEN [ModifiedBy] = @OldId THEN @NewId ELSE [ModifiedBy] END,
                        [UserId] = CASE WHEN [UserId] = @OldId THEN @NewId ELSE [UserId] END
                WHERE   [ModifiedBy] = @OldId OR
                        [UserId] = @OldId";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new { OldId = oldId, NewId = newId });
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
                return false;
            }

            return true;
        }
    }
}
