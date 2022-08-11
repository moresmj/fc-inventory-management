using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Stores.PhysicalCount
{
    public class ApprovePhysicalCountSearchDTO
    {

        public RequestStatusEnum?[] RequestStatus { get; set; }
    }
}
