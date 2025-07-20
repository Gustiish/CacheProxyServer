using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace CacheProxyServer
{
    internal class CacheService<T> : ICacheService<T> where T : class
    {
        private readonly IDistributedCache distributedCache;
        private readonly LiteDatabase liteDatabase;
        private readonly ILiteCollection<BsonDocument> collection;
       

        public CacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
            var liteDbFilepath = GetFilePath();
            this.liteDatabase = new LiteDatabase(liteDbFilepath);
        }

        public async Task<T?> GetAsync(string key)
        {
            var bytes = await distributedCache.GetAsync(key);
            if (bytes != null)
            {
                string Json = Encoding.UTF8.GetString(bytes);
                return System.Text.Json.JsonSerializer.Deserialize<T>(Json);
            }

            using var db = new LiteDatabase(GetFilePath());
            var value = db.GetCollection<T>().FindById(key);

            return System.Text.Json.JsonSerializer.Deserialize<T>(value);


        }

        public Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(string key, T value, TimeSpan? expiry = null)
        {
            throw new NotImplementedException();
        }

        private string GetFilePath()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var filePath = Path.Combine(appdata, "CacheProxyServer", "cache.db");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            return filePath;
        }
    }
}
