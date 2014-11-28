using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System;

namespace Bennett.AbroadAdvisor.Models
{
    public class PingModel
    {
        public static string Ping()
        {
            const string sql = "SELECT version() AS version";
            string result = String.Empty;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
                {
                    connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;

                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        connection.Open();
                        result = command.ExecuteScalar() as String;
                    }
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                throw e;
            }

            return result;
        }
    }
}
