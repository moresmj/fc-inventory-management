using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using log4net.Repository.Hierarchy;

namespace FC.Batch.Helpers.FileLog
{
    public class Log4netProvider
    {
        public ILog4netWriter GetFileLogger(string categoryName, string logPath, string configPath, string loggerName, string appenderName)
        {
            var repository = this.GetHierarchyRepository(categoryName);

            log4net.Config.XmlConfigurator.Configure(repository, new System.IO.FileInfo(configPath));

            var appender = (log4net.Appender.FileAppender)repository
                .GetLogger(loggerName, ((log4net.Repository.Hierarchy.Hierarchy)repository).LoggerFactory)
                .GetAppender(appenderName);

            if (appender != null)
            {
                appender.File = logPath;
                appender.ActivateOptions();

                var logger = log4net.LogManager.GetLogger(categoryName, loggerName);
                return new Log4netWriter(logger);
            }

            return null;
        }

        private Hierarchy GetHierarchyRepository(string categoryName)
        {
            if (log4net.Core.LoggerManager.RepositorySelector.ExistsRepository(categoryName))
            {
                return log4net.Core.LoggerManager.GetRepository(categoryName) as Hierarchy;
            }

            return log4net.Core.LoggerManager.CreateRepository(categoryName) as Hierarchy;
        }
    }
}
