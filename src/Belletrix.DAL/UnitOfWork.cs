using System;
using System.Data;
using System.Data.SqlClient;

namespace Belletrix.DAL
{
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
