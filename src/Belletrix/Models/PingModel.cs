using Belletrix.Core;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Belletrix.Models
{
    public class PingModel
    {
        public static async Task<string> Ping()
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
                        await connection.OpenAsync();
                        result = await command.ExecuteScalarAsync() as String;
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
