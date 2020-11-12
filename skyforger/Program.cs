using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            CreateWebHostBuilder(args, environment).Build().Run();
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string environment) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel((config, t) =>
                {
                    var certfile = config.Configuration["Ssl:CertFile"];
                    t.ListenAnyIP(5000);
                    t.ListenAnyIP(443, lo => { lo.UseHttps(certfile, config.Configuration["CERT_PASSWORD"]); });
                    
                })
                .UseStartup<Startup>();
    }
}
