using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public interface ILogisticsDashboardService
    {
        
        object DashboardSummary(IMapper mapper);

        object GetPendingDeliveryOrders(IMapper mapper);

        object GetPendingDeliverySales();

        object GetPendingPickUpClientReturns();

        object GetPendingPickUpRTV();

        IEnumerable<STDeliveryDTO> DeliveryRecords(IMapper mapper, int? id = null, IdTypeEnum? idType = null);


    }
}
