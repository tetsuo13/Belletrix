using System;

namespace Belletrix.Core
{
    internal interface ICacheService
    {
        T Get<T>(string cacheId, Func<T> getItemCallback) where T : class;
        void Set<T>(string cacheId, T item) where T : class;
        void Clear();
    }
}
