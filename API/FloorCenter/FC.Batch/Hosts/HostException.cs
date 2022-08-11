using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Hosts
{
    public class HostException
        : Exception
    {
        public HostException(string name, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Name = name;
        }

        public virtual string Name { get; }
    }
}
