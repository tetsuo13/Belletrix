using Dapper;
using StackExchange.Exceptional;
using System;
using System.Threading.Tasks;
using System.Web;

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
            const string sql = "SELECT @@version AS Version";

            try
            {
                return await UnitOfWork.Context().ExecuteScalarAsync<string>(sql);
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return string.Empty;
        }
    }
}
