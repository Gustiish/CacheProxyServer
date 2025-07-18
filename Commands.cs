using Cocona;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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


            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<Forwarder>(prop =>
            {
                var factory = prop.GetRequiredService<IHttpClientFactory>();
                var client = factory.CreateClient();
                client.BaseAddress = new Uri(origin);
                IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

                return new Forwarder(client, cache);
            });


            var app = builder.Build();

            Console.WriteLine($"URL is set to {origin}");

            app.MapGet("{**catchAll}", async (HttpContext context, Forwarder forwarder) =>
            {            
                if (forwarder.CheckCache(context.Request, out HttpResponseMessage cachedBody))
                {
                   
                    await UpdateHttpContentAndHeaders(cachedBody, ref context);
                    DisplayContextHeaders(context);

                }
                else
                {
                    HttpResponseMessage response = await forwarder.ForwardNewRequestAsync(context.Request); //This method forwards and also caches the conent body as string

                    await UpdateHttpContentAndHeaders(response, ref context);

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

        private static async Task UpdateHttpContentAndHeaders(HttpResponseMessage response, ref HttpContext context)
        {
            context.Response.StatusCode = (int)response.StatusCode;

            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in response.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("Transfer-Encoding");

            await using Stream content = await response.Content.ReadAsStreamAsync();
            await content.CopyToAsync(context.Response.Body);
        }


        [Command("clear-cache", Description = "Clear the cache")]
        public void ClearCache()
        {
            throw new NotImplementedException("Cache clearing functionality is not implemented yet.");
        }
        [Command("list-cache", Description = "List the cache contents")]
        public void ListCache()
        {
            throw new NotImplementedException("Cache listing functionality is not implemented yet.");
        }

        [Command("version", Description = "Display the version of the application")]
        public void DisplayVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            Console.WriteLine($"CacheProxyServer version {version}");


        }
    }
}
