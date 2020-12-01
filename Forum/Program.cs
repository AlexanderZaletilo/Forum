using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Forum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(builder =>
                    builder.ClearProviders()
                           .AddAzureWebAppDiagnostics()
                           .AddConsole()
                           .AddProvider(new FileLoggerProvider("all.txt", LogLevel.Information))
                           .AddProvider(new FileLoggerProvider("errors.txt", LogLevel.Error)));
    }
}
