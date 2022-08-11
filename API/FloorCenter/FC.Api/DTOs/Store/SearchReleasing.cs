using FC.Core.Domain.Common;
using System;

namespace FC.Api.DTOs.Store
{
    public class SearchReleasing : BaseGetAll
    {

        public int? WarehouseId { get; set; }

        public string PONumber { get; set; }

        public string DRNumber { get; set; }

        public string WHDRNumber { get; set; }

        public DateTime? DeliveryDateFrom { get; set; }

        public DateTime? DeliveryDateTo { get; set; }

        public DeliveryTypeEnum?[] DeliveryType { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

    }
}
