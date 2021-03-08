using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.IO;
using System.Net;

namespace OcelotApiGateway {
    public class Program {
        public static void Main(string[] args) {
            new WebHostBuilder()
                           .UseKestrel(options => {
                               var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));
                               var httpsPort = configuration.GetValue("ASPNETCORE_HTTPS_PORT", 443);
                               string path = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path");
                               string pass = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password");

                               options.Listen(IPAddress.Any, httpsPort, listenOptions => {
                                   listenOptions.UseHttps(path, pass);
                               });
                           }
                           )
                           .UseUrls("https://0.0.0.0:443")
                           .UseContentRoot(Directory.GetCurrentDirectory())
                           .ConfigureAppConfiguration((hostingContext, config) => {
                               config
                                   .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                                   .AddJsonFile("appsettings.json", true, true)
                                   .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                                   .AddJsonFile("ocelot.json")
                                   .AddEnvironmentVariables();
                           })
                           .ConfigureServices(s => {
                               s.AddOcelot();
                           })
                           .ConfigureLogging((hostingContext, logging) => {
                               //add your logging
                           })
                           .UseIISIntegration()
                           .Configure(app => {
                               app.UseOcelot().Wait();
                           })
                           .Build()
                           .Run();
        }
    }
}
