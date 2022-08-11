using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Batch.Scopes
{
    public interface IScopedExecuteService
    {
        string ConfigSection { get; }

        void Execute(CancellationToken cancellationToken, dynamic state);
    }
}
