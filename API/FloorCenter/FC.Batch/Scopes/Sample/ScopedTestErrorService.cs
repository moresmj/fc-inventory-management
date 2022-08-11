using FC.Batch.Helpers.DBContext;
using FC.Batch.Helpers.DBContext.Extension;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Batch.Scopes.Sample
{
    public interface IScopedTestErrorService : ITestScopedExecuteService
    {
    }

    internal class ScopedTestErrorService
        : IScopedTestErrorService
    {
        private DataContext _context;

        public ScopedTestErrorService(DataContext context)
        {
            _context = context;
        }

        public string ConfigSection => "Batch:Task:TestError";

        public void Execute(CancellationToken cancellationToken, dynamic state)
        {
            //Codes
            //var stocks = _context.WHStocks.FindAll();
            throw new NotImplementedException();
        }
    }
}
