using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.ModifyTonality
{
    public class SearchModifyTonality : BaseGetAll
    {

        public int? WarehouseId { get; set; }

        public RequestStatusEnum?[] RequestStatus { get; set; }

        public DateTime? RequestDateFrom { get; set; }

        public DateTime? RequestDateTo { get; set; }
    }
}
