using FC.Batch.Helpers.FileLog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers.FileLog
{
    public class SystemExceptionFileLogger
    {
        private string CategoryName { get; } = "_SystemExceptionLog";

        public SystemExceptionFileLogger(IServiceProvider services, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            var logConfiguration = configuration.GetSection("Log4net").Get<Log4netConfiguration>();

            var log4netProvider = services.GetRequiredService<Log4netProvider>();
            var logPath = logConfiguration.LogPath + $"{OperateFileLogger.CategoryName}\\error\\";
            var configPath = hostingEnvironment.ContentRootPath + "\\log4net.config";

            this._logger = log4netProvider.GetFileLogger(this.CategoryName, logPath, configPath, "systemExceptionLogger", "systemExceptionAppender");
        }

        private ILog4netWriter _logger { get; }

        public void Log(Exception exception)
        {
            _logger.Error(exception.ToString());
        }
    }
}
