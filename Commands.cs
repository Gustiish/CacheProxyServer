using Cocona;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;

namespace CacheProxyServer
{
    internal class Commands
    {
        
        [Command("start", Description = "Start the application and set the port and url")]
        public async Task SetOriginAndPort([Option] int port, [Option] string origin)
        {

            var builder = WebApplication.CreateBuilder();
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(port);
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<Proxy>(proxy =>
            {
             
                var httpClient = proxy.GetRequiredService<HttpClient>();
                httpClient.BaseAddress = new Uri(origin);
                var cache = proxy.GetRequiredService<IDistributedCache>();
                return new Proxy(httpClient, cache);
            });

            var app = builder.Build();

            Console.WriteLine($"URL is set to {origin}");
            app.MapGet("{**catchAll}", async (HttpContext context, Proxy _forwarder) =>
            {
                if (_forwarder.CheckCache(context.Request, out HttpResponseMessage cachedBody))
                {
                    await UpdateHttpContentAndHeaders(cachedBody, context);
                    DisplayContextHeaders(context);
                }
                else
                {
                    HttpResponseMessage response = await _forwarder.ForwardNewRequestAsync(context.Request); //This method forwards and also caches the conent body as string
                    await UpdateHttpContentAndHeaders(response, context);
                    DisplayContextHeaders(context);
                }
            });

            await app.RunAsync();


        }

        private static void DisplayContextHeaders(HttpContext context)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var header in context.Response.Headers)
            {
                Console.WriteLine($"Headerkey: {header.Key}\n Headervalue: {header.Value}\n------");
            }
        }

        private static async Task UpdateHttpContentAndHeaders(HttpResponseMessage response, HttpContext context)
        {
            context.Response.StatusCode = (int)response.StatusCode;

            context.Response.Headers.Clear();
            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            Stream content = response.Content.ReadAsStream();
            await content.CopyToAsync(context.Response.Body);
        }

        [Command("list-cache", Description = "List all cached items")]
        public static async Task ListCacheAsync()
        {
           
        }


    }
}
