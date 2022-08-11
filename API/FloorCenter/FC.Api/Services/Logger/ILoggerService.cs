using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Logger
{
    public interface ILoggerService
    {

        void WriteLogs(string controller, string action, string exceptionType, string logErrorMessage, string jwtToken);

    }
}
