using Belletrix.Core;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Belletrix.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbConnection context;

        public UnitOfWork(string connectionString)
        {
            SqlConnection connection = new SqlConnection(Connections.Database.Dsn);
            connection.Open();
            context = new ProfiledDbConnection(connection, MiniProfiler.Current);
        }

        public DbConnection Context()
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
