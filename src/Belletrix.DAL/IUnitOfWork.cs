using System;
using System.Data.SqlClient;

namespace Belletrix.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        SqlCommand CreateCommand();
    }
}
