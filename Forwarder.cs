using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var path = request.Path + request.QueryString;
            
            var forwardRequest = new HttpRequestMessage(HttpMethod.Get, path);

            foreach (var header in request.Headers)
            {
                forwardRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            var response = await _httpClient.SendAsync(forwardRequest);

            //Create cache method here
            return response;


        }
    }
}
