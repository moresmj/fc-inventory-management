using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FC.Batch.Hosts;
using FC.Batch.Helpers.FileLog;

namespace FC.Batch.Helpers
{
    public class ViewErrorLogProvider
        : ILoggerProvider
    {
        public ViewErrorLogProvider(IServiceProvider services)
        {
            this.Services = services;
        }

        public IServiceProvider Services { get; }

        public ILogger CreateLogger(string categoryName)
        {
            var systemExceptionFileLogger = this.Services.GetRequiredService<SystemExceptionFileLogger>();
            return new ViewErrorLogger(this.Services, systemExceptionFileLogger);
        }

        public void Dispose()
        {

        }
    }

    public class ViewErrorLogger
        : ILogger
    {
        public ViewErrorLogger(IServiceProvider services, SystemExceptionFileLogger systemExceptionFileLogger)
        {
            this.Services = services;
            this.SystemExceptionFileLogger = systemExceptionFileLogger;
        }

        public IServiceProvider Services { get; }

        protected SystemExceptionFileLogger SystemExceptionFileLogger { get; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LogLevel.Error || logLevel == LogLevel.Critical;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel) && (exception as HostException) == null)
            {
                return;
            }

            this.Services.GetRequiredService<ViewError>().Log(exception);

            if (this.SystemExceptionFileLogger != null)
            {
                this.SystemExceptionFileLogger.Log(exception);
            }
        }
    }
}
