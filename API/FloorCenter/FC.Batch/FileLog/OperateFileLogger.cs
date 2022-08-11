using FC.Batch.Helpers.FileLog;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace FC.Batch.Helpers.FileLog
{
    public class OperateFileLogger
    {
        internal static string CategoryName { get; } = "_OperationLog";

        public OperateFileLogger(IServiceProvider services, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            var logConfiguration = configuration.GetSection("Log4net").Get<Log4netConfiguration>();

            var log4netProvider = services.GetRequiredService<Log4netProvider>();
            var logPath = logConfiguration.LogPath + $"{CategoryName}\\info\\operate.log";
            var configPath = hostingEnvironment.ContentRootPath + "\\log4net.config";

            this._logger = log4netProvider.GetFileLogger(CategoryName, logPath, configPath, "operateLogger", "operateAppender");
        }

        private ILog4netWriter _logger { get; }

        public void Log(string operationName, OperationType operation, DateTime start, DateTime end)
        {
            if (_logger != null)
            {
                if (operation == OperationType.Started)
                {
                    this.Log($"{start.ToString()},, {operationName} started.");
                }
                else if (operation == OperationType.Done)
                {
                    this.Log($"{start.ToString()}, {end.ToString()}, {operationName} finished.");
                }
                else if (operation == OperationType.Running)
                {
                    this.Log($"{start.ToString()}, {end.ToString()}, {operationName} is already running.");
                }
                else if (operation == OperationType.Error)
                {
                    this.Log($"{start.ToString()}, {end.ToString()}, {operationName} finished with error.");
                }
            }
        }

        public void Log(string message)
        {
            if (_logger != null)
            {
                _logger.Info(message);
            }
        }
    }
}
