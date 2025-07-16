using Microsoft.AspNetCore.Http;

namespace CacheProxyServer
{
    internal class Forwarder
    {
        private readonly HttpClient _httpClient;


        public Forwarder(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> ForwardRequestAsync(HttpRequest request)
        {
            var path = new Uri(_httpClient.BaseAddress, request.Path + request.QueryString);

            var forwardRequest = new HttpRequestMessage(HttpMethod.Get, path);


            var response = await _httpClient.SendAsync(forwardRequest);

            //Create cache method here
            return response;


        }
    }
}
