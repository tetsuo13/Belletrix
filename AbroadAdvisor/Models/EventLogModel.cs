using Npgsql;
using System;

namespace Bennett.AbroadAdvisor.Models
{
    public class EventLogModel
    {
        public enum EventType
        {
            AddStudent,
            EditStudent,
            AddUser,
            EditUser
        };

        public int Id { get; set; }
        public DateTime EventDate { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public int ReferenceId { get; set; }
        public string Action { get; set; }

        public List<EventLogModel> GetEvents()
        {
            List<EventLogModel> events = new List<EventLogModel>();
            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT      id, date, user_id, type, action
                        FROM        event_log
                        ORDER BY    date DESC";
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string type = "";

                            switch (reader.Int32(reader.GetOrdinal("type")))
                            {
                                case EventType.AddStudent:
                                    type = "Added"
                            }
                            StudentModel student = new StudentModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                LastName = reader.GetString(reader.GetOrdinal("last_name"))
                            };
                        }
                    }
                }
            }

            return events;
        }

        public static void Add(NpgsqlConnection connection, int userId, EventType eventType, string action)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO event_log
                    (
                        date, user_id, type, action
                    )
                    VALUES
                    (
                        @Date, @UserId, @Type, @Action
                    )";

                command.Parameters.Add("@Date", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                command.Parameters.Add("@UserId", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;
                command.Parameters.Add("@Type", NpgsqlTypes.NpgsqlDbType.Integer).Value = (int)eventType;
                command.Parameters.Add("@Action", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = action;

                command.ExecuteNonQuery();
            }
        }
    }
}
