using Cocona;
using Cocona.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace CacheProxyServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            


            CoconaAppBuilder builder = CoconaApp.CreateBuilder();
         
            CoconaApp coconaApp = builder.Build();
            
            coconaApp.AddCommands<Commands>();

            await coconaApp.RunAsync();


        }
    }
}
