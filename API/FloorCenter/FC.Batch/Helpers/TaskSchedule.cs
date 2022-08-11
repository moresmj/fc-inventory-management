using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers
{
    public class TaskSchedule
    {
        public string Schedule { get; set; }

        public bool Active { get; set; }

        public bool EnabledStartupRun { get; set; }


        public DateTime? LastExecutedDate { get; internal set; }

        public bool IsScopeRunning { get; internal set; }
    }
}
