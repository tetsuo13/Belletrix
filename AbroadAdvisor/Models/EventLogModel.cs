using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public static void Add(NpgsqlConnection connection, NpgsqlTransaction transaction, int userId,
            EventType eventType, string action)
        {
            const string sql = @"
                INSERT INTO event_log
                (
                    date, user_id, type, action
                )
                VALUES
                (
                    @Date, @UserId, @Type, @Action
                )";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Transaction = transaction;
                command.Parameters.Add("@Date", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Now.ToUniversalTime();
                command.Parameters.Add("@UserId", NpgsqlTypes.NpgsqlDbType.Integer).Value = userId;
                command.Parameters.Add("@Type", NpgsqlTypes.NpgsqlDbType.Integer).Value = (int)eventType;
                command.Parameters.Add("@Action", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = action;
                command.ExecuteNonQuery();
            }
        }
    }
}
