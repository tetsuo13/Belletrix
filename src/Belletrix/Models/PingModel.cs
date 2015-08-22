using Belletrix.Core;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Belletrix.Models
{
    public class PingModel
    {
        public static async Task<string> Ping()
        {
            const string sql = "SELECT @@version";
            string result = String.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
                {
                    using (SqlCommand command = connection.CreateCommand())
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
