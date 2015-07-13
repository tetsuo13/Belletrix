using Belletrix.Core;
using Npgsql;
using System;

namespace Belletrix.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly string ConnectionString;
        private NpgsqlConnection context;
        private NpgsqlTransaction transaction;

        public UnitOfWork(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public NpgsqlConnection DbContext
        {
            get
            {
                if (context == null)
                {
                    context = new NpgsqlConnection(ConnectionString);
                    context.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                    context.Open();
                    transaction = context.BeginTransaction();
                }
                return context;
            }
        }

        public void SaveChanges()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    transaction = null;
                }

                if (context != null)
                {
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
