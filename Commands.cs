using Cocona;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                return new Forwarder(client);
            });


            var app = builder.Build();

            Console.WriteLine($"URL is set to {origin}");

            app.MapGet("{**catchAll}", async (HttpContext context, Forwarder forwarder) =>
            {
                HttpResponseMessage response = await forwarder.ForwardRequestAsync(context.Request);

                var content = await response.Content.ReadAsStringAsync();
                return Results.Ok(content);
            });

            await app.RunAsync();

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
