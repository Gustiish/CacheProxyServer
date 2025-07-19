using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheProxyServer
{
    internal class Proxy
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;

        public Proxy(HttpClient httpClient, IDistributedCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<HttpResponseMessage> ForwardNewRequestAsync(HttpRequest request)
        {
            var path = new Uri(_httpClient.BaseAddress, request.Path + request.QueryString);

            var forwardRequest = new HttpRequestMessage(HttpMethod.Get, path);

            HttpResponseMessage response = await _httpClient.SendAsync(forwardRequest);

           
            string key = request.Path + request.QueryString;
            //Add the response to the cache here


            string body = await response.Content.ReadAsStringAsync();

            HttpResponseMessage newResponse = new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"), 
            };

            newResponse.Headers.Clear();
            newResponse.Headers.Add("X-Cache", "MISS");

            return newResponse;
        }


        public bool CheckCache(HttpRequest request, out HttpResponseMessage cachedResponse)
        {
            var path = new Uri(_httpClient.BaseAddress, request.Path + request.QueryString);
            string key = request.Path + request.QueryString;
            if (_cache.TryGetValue(key, out cachedResponse))
            {
                cachedResponse.Headers.Clear();
                cachedResponse.Headers.Add("X-Cache", "HIT");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearCache()
        {
            
        }

       
    }
}
