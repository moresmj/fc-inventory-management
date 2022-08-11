using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FC.Batch.Helpers.FileLog;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Batch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                }).UseSetting("detailedErrors", "true")
                .ConfigureServices(services => services.AddSingleton<Helpers.ViewError>())
                .ConfigureServices(services => services.AddSingleton<Log4netProvider>())
                .ConfigureServices(services => services.AddSingleton<OperateFileLogger>())
                .ConfigureServices(services => services.AddSingleton<SystemExceptionFileLogger>())
                .ConfigureLogging(builder => builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>())
                .ConfigureLogging(builder => builder.Services.AddSingleton<ILoggerProvider, Helpers.ViewErrorLogProvider>())
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .Build();
    }
}
