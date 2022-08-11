using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers
{
    public class BatchConfiguration
    {
        public int Repeat { get; set; } = 1;

        public bool RunTest { get; set; }

        public string[] TargetDebug { get; set; }
    }
}
