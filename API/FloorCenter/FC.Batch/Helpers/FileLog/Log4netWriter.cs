using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net.Core;
using Microsoft.Extensions.Configuration;

namespace FC.Batch.Helpers.FileLog
{
    public interface ILog4netWriter
    {
        void Error(object message);
        void Error(object message, Exception exception);
        void Info(object message);
        void Info(object message, Exception exception);
    }

    public class Log4netWriter : ILog4netWriter
    {
        public Log4netWriter(log4net.ILog logger)
        {
            this.Logger = logger;
        }

        private log4net.ILog Logger { get; }

        public void Error(object message)
        {
            this.Logger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            this.Logger.Error(message);
        }

        public void Info(object message)
        {
            this.Logger.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            this.Logger.Info(message, exception);
        }
    }
}
