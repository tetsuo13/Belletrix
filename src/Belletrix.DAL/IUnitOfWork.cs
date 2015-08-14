using System;
using System.Data.SqlClient;

namespace Belletrix.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        SqlConnection DbContext { get; }
        void SaveChanges();
    }
}
