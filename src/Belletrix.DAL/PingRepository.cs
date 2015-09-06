using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Belletrix.DAL
{
    public class PingRepository : IPingRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public PingRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<string> Ping()
        {
            const string sql = "SELECT @@version";
            string result = String.Empty;

            try
            {
                using (SqlCommand command = UnitOfWork.CreateCommand())
                {
                    command.CommandText = sql;
                    result = await command.ExecuteScalarAsync() as String;
                }
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
            }

            return result;
        }
    }
}
