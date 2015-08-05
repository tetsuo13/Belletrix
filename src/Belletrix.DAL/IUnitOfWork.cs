using Npgsql;
using System;

namespace Belletrix.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        NpgsqlConnection DbContext { get; }
        void SaveChanges();
    }
}
