using CacheProxyServer.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheProxyServer
{
    internal class CacheHttpDTO : ICacheId
    {
        public int Id { get; set; }
        public int StatusCode { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Headers { get; set; }


    }
}
