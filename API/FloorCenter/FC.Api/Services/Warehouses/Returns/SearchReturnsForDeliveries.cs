using FC.Api.DTOs;
using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.Services.Warehouses.Returns
{
    public class SearchReturnsForDeliveries : BaseGetAll
    {

        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDateTo { get; set; }

        public string TransactionNo { get;  set; }

        public DeliveryStatusEnum?[] DeliveryStatus { get; set; }
    }
}
