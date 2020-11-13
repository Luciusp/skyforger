using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace skyforger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // Tailor environment variable (empty string catch)
            if (string.IsNullOrWhiteSpace(environment))
            {
                environment = "development";
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.ToLower()}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            CreateHostBuilder(args, config).Build().Run();
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot config) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:5000");
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
