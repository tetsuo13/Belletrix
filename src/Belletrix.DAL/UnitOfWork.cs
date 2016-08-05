using System;
using System.Data;
using System.Data.SqlClient;

namespace Belletrix.DAL
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// If there is an uncaught exception in the application anywhere, the
    /// Dispose() method doesn't end up rolling back the transaction. When the
    /// user revisits the web site they will end up with the following error:
    /// 
    /// <code>
    /// Npgsql.NpgsqlException:
    /// current transaction is aborted, commands ignored until end of transaction block
    /// Severity: ERROR
    /// Code: 25P02
    /// </code>
    /// 
    /// This can be fixed by restarting the application on the server.
    /// Ultimately this is an indication of bad design on the UnitOfWork
    /// implementation with regard to transaction scope.
    /// </remarks>
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection context;

        public UnitOfWork(string connectionString)
        {
            context = new SqlConnection(connectionString);
            context.Open();
        }

        public IDbConnection Context()
        {
            return context;
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Close();
                    context.Dispose();
                    context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
