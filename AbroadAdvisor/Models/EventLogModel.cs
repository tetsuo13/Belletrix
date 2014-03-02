using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Bennett.AbroadAdvisor.Models
{
    public class EventLogModel
    {
        private const string CacheId = "EventLog";

        public enum EventType
        {
            AddStudent,
            EditStudent,
            AddUser,
            EditUser
        };

        public int Id { get; set; }
        public DateTime EventDate { get; set; }
        
        /// <summary>
        /// References an <see cref="EventType"/> element.
        /// </summary>
        public int Type { get; set; }
        
        public string Action { get; set; }

        public UserModel ModifiedBy { get; set; }

        public StudentModel Student { get; set; }

        public UserModel User { get; set; }

        public string RelativeDate { get; set; }

        public static List<EventLogModel> GetEvents()
        {
            ApplicationCache cacheProvider = new ApplicationCache();
            List<EventLogModel> events = cacheProvider.Get(CacheId, () => new List<EventLogModel>());

            if (events.Count == 0)
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT          e.id, e.date, e.modified_by,
                                        e.student_id, e.user_id, e.type,
                                        e.action, u.first_name, u.last_name,
                                        s.first_name AS student_first_name,
                                        s.last_name AS student_last_name,
                                        us.first_name AS user_first_name,
                                        us.last_Name AS user_last_name
                        FROM            event_log e
                        INNER JOIN      users u ON
                                        modified_by = u.id
                        LEFT OUTER JOIN students s ON
                                        e.student_id = s.id
                        LEFT OUTER JOIN users us ON
                                        e.user_id = us.id
                        ORDER BY        date DESC
                        LIMIT           6";

                        connection.Open();

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserModel modifiedBy = new UserModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("modified_by")),
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_name"))
                                };

                                int ord = reader.GetOrdinal("action");
                                string action = null;
                                if (!reader.IsDBNull(ord))
                                {
                                    action = reader.GetString(ord);
                                }

                                StudentModel student = null;
                                ord = reader.GetOrdinal("student_id");
                                if (!reader.IsDBNull(ord))
                                {
                                    student = new StudentModel()
                                    {
                                        Id = reader.GetInt32(ord),
                                        FirstName = reader.GetString(reader.GetOrdinal("student_first_name")),
                                        LastName = reader.GetString(reader.GetOrdinal("student_last_name"))
                                    };
                                }

                                UserModel user = null;
                                ord = reader.GetOrdinal("user_id");
                                if (!reader.IsDBNull(ord))
                                {
                                    user = new UserModel()
                                    {
                                        Id = reader.GetInt32(ord),
                                        FirstName = reader.GetString(reader.GetOrdinal("user_first_name")),
                                        LastName = reader.GetString(reader.GetOrdinal("user_last_name"))
                                    };
                                }

                                EventLogModel eventLog = new EventLogModel()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    EventDate = DateTimeFilter.UtcToLocal(reader.GetDateTime(reader.GetOrdinal("date"))),
                                    ModifiedBy = modifiedBy,
                                    Student = student,
                                    User = user,
                                    Type = reader.GetInt32(reader.GetOrdinal("type")),
                                    Action = action
                                };

                                eventLog.RelativeDate = CalculateRelativeDate(eventLog.EventDate.ToUniversalTime());

                                events.Add(eventLog);
                            }

                            cacheProvider.Set(CacheId, events);
                        }
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <seealso href="https://stackoverflow.com/a/1248">
        /// How do I calculate relative time?
        /// </seealso>
        public static string CalculateRelativeDate(DateTime date)
        {
            const int Second = 1;
            const int Minute = 60 * Second;
            const int Hour = 60 * Minute;
            const int Day = 24 * Hour;
            const int Month = 30 * Day;

            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * Minute)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * Minute)
            {
                return "a minute ago";
            }
            if (delta < 45 * Minute)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * Minute)
            {
                return "an hour ago";
            }
            if (delta < 24 * Hour)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * Hour)
            {
                return "yesterday";
            }
            if (delta < 30 * Day)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * Month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }

            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        public void AddStudentEvent(NpgsqlConnection connection, int modifiedBy, int studentId, EventType eventType)
        {
            connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO event_log
                    (
                        date, modified_by, student_id, type
                    )
                    VALUES
                    (
                        @Date, @ModifiedBy, @StudentId, @Type
                    )";

                command.Parameters.Add("@Date", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                command.Parameters.Add("@ModifiedBy", NpgsqlTypes.NpgsqlDbType.Integer).Value = modifiedBy;
                command.Parameters.Add("@Type", NpgsqlTypes.NpgsqlDbType.Integer).Value = (int)eventType;
                command.Parameters.Add("@StudentId", NpgsqlTypes.NpgsqlDbType.Integer).Value = studentId;

                command.ExecuteNonQuery();

                ApplicationCache cacheProvider = new ApplicationCache();
                List<EventLogModel> events = cacheProvider.Get(CacheId, () => new List<EventLogModel>());
                events.Add(this);
                cacheProvider.Set(CacheId, events);
            }
        }
    }
}
