using Npgsql;
using System;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Models
{
    public class PingModel
    {
        public static string Ping()
        {
            string result = String.Empty;

            string dsn = ConfigurationManager.ConnectionStrings["Production"].ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(dsn))
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
