using FC.Batch.Hosts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FC.Batch.Helpers.FileLog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Batch.Helpers.FileLog
{
    public class FileLogger : ILogger
    {
        public FileLogger(string categoryName, Log4netProvider logProvider, IServiceProvider services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            this.LogProvider = logProvider;
            this.Services = services;
            this.Configuration = configuration;
            this.HostingEnvironment = hostingEnvironment;
        }

        private Log4netProvider LogProvider { get; }

        public IServiceProvider Services { get; }

        protected IConfiguration Configuration { get; }

        protected IHostingEnvironment HostingEnvironment { get; }

        private ILog4netWriter _logger;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Error;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            var error = exception as HostException;
            if (error != null)
            {
                var logConfiguration = new Log4netConfiguration();
                this.Configuration.GetSection("Log4net").Bind(logConfiguration);

                var logPath = $"{logConfiguration.LogPath}{error.Name}\\error\\";
                var configPath = this.HostingEnvironment.ContentRootPath + $"\\{logConfiguration.ConfigPath}";

                try
                {
                    if (_logger == null)
                    {
                        _logger = this.LogProvider.GetFileLogger(error.Name, logPath, configPath, logConfiguration.LoggerName, logConfiguration.AppenderName);
                    }
                }
                catch { }

                if (_logger != null)
                {
                    _logger.Error(exception.ToString());
                }
            }
        }
    }
}
