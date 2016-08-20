using System;
using System.Data.Common;

namespace Belletrix.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        DbConnection Context();
    }
}
