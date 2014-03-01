using System;

namespace Bennett.AbroadAdvisor.Core
{
    internal interface ICacheService
    {
        T Get<T>(string cacheId, Func<T> getItemCallback) where T : class;
        void Set<T>(string cacheId, T item) where T : class;
    }
}
