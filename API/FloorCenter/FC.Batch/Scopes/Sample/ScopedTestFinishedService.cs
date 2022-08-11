using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Batch.Scopes.Sample
{
    public interface IScopedTestFinishedService : ITestScopedExecuteService
    {
    }

    public class ScopedTestFinishedService
        : IScopedTestFinishedService
    {
        public string ConfigSection => "Batch:Task:TestFinish";

        public void Execute(CancellationToken cancellationToken, dynamic state)
        {
            
        }
    }
}
