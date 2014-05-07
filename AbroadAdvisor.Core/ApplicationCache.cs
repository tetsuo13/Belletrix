using System;
using System.Collections.Generic;
using System.Web;

namespace Bennett.AbroadAdvisor.Core
{
    public class ApplicationCache : ICacheService
    {
        public T Get<T>(string cacheId, Func<T> getItemCallback) where T : class
        {
            T item = HttpRuntime.Cache.Get(cacheId) as T;

            if (item == null)
            {
                item = getItemCallback();
                HttpRuntime.Cache.Insert(cacheId, item);
            }

            return item;
        }

        public void Set<T>(string cacheId, T item) where T : class
        {
            HttpRuntime.Cache.Insert(cacheId, item);
        }

        public void Clear()
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            Dictionary<string, object> items = new Dictionary<string, object>();

            while (enumerator.MoveNext())
            {
                items.Add(enumerator.Key.ToString(), enumerator.Value);
            }

            foreach (string key in items.Keys)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }
    }
}
