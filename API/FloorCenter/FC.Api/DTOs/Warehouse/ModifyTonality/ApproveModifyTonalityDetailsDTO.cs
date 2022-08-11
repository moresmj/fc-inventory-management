using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.ModifyTonality
{
    public class ApproveModifyTonalityDetailsDTO
    {

        public int Id { get; set; }

        public int? WHModifyItemTonalityId { get; set; }

        public int? ItemId { get; set; }

        public int? OldItemId { get; set; }

        public int? StClientDeliveryId { get; set; }

        public int? StShowroomDeliveryId { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public DateTime? DateUpdated { get; set; }

        public string Remarks { get; set; }

    }
}
