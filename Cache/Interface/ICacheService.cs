using CacheProxyServer.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProxyServer
{
    internal interface ICacheService<T> where T : class, ICacheId
    {
        Task<T> GetAsync(string key);
        Task SetAsync(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
    }
}
