using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace FC.Batch.Helpers.FileLog
{
    public class FileLoggerProvider : ILoggerProvider
    {
        public FileLoggerProvider(IServiceProvider services)
        {
            this.Services = services;
        }

        public IServiceProvider Services { get; }

        public ILogger CreateLogger(string categoryName)
        {
            var log4netProvider = this.Services.GetRequiredService<Log4netProvider>();
            var configuration = this.Services.GetRequiredService<IConfiguration>();
            var hostingEnvironment = this.Services.GetRequiredService<IHostingEnvironment>();

            return new FileLogger(categoryName, log4netProvider, this.Services, configuration, hostingEnvironment);
        }

        public void Dispose()
        {

        }
    }
}
