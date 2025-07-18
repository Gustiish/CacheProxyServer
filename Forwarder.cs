using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

namespace CacheProxyServer
{
    internal class Forwarder
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;


        public Forwarder(HttpClient httpClient, IMemoryCache cache)
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
            _cache.Set(key, response, TimeSpan.FromMinutes(5));
            Console.WriteLine($"X-Cache: MISS");

            string body = await response.Content.ReadAsStringAsync();

            HttpResponseMessage newResponse = new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"), 
            };

            foreach (var headers in response.Headers)
            {
                newResponse.Headers.TryAddWithoutValidation(headers.Key, headers.Value);
            }

            foreach (var headers in response.Content.Headers)
            {
                newResponse.Content.Headers.TryAddWithoutValidation(headers.Key, headers.Value);
            }

            return newResponse;
        }


        public bool CheckCache(HttpRequest request, out HttpResponseMessage cachedResponse)
        {
            var path = new Uri(_httpClient.BaseAddress, request.Path + request.QueryString);
            string key = request.Path + request.QueryString;
            if (_cache.TryGetValue(key, out cachedResponse))
            {
                Console.WriteLine($"X-Cache: HIT");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
