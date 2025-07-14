using Cocona;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace CacheProxyServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
            var builder = CoconaApp.CreateBuilder();
            var app = builder.Build();
            app.AddCommands<Commands>();

            Console.WriteLine("App is running");

            await app.RunAsync();


        }
    }
}
