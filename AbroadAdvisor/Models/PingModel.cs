using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;

namespace Bennett.AbroadAdvisor.Models
{
    public class PingModel
    {
        public static string Ping()
        {
            string result = String.Empty;

            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT version() AS version";
                    connection.Open();

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetString(reader.GetOrdinal("version"));
                        }
                    }
                }
            }

            return result;
        }
    }
}
