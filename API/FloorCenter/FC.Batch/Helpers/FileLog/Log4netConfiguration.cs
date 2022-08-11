using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers.FileLog
{
    public class Log4netConfiguration
    {
        public string ConfigPath { get; set; }

        public string LogPath { get; set; }

        public string LoggerName { get; set; }

        public string AppenderName { get; set; }
    }
}
