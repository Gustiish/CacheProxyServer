using Cocona;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace CacheProxyServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            var builder = CoconaApp.CreateBuilder();
            var app = builder.Build();
            app.AddCommands<Commands>();

            app.RunAsync();


        }
    }
}
