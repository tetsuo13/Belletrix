using System;
using System.Data;

namespace Belletrix.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Context();
    }
}
