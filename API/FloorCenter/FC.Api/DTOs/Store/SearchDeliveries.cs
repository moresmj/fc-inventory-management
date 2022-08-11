using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{
    public class SearchDeliveries : BaseGetAll
    {
        public string TransactionNo { get; set; }

        public string PONumber { get; set; }

        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDateTo { get; set; }

        public DeliveryStatusEnum?[] DeliveryStatus { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }
    }
}
