using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{
    public class OrderSearch : BaseGetAll
    {

        public int? StoreId { get; set; }

        public int? WarehouseId { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public string PONumber { get; set; }

        public string transactionNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateTo { get; set; }

        public RequestStatusEnum?[] RequestStatus { get; set; }

        public OrderStatusEnum?[] OrderStatus { get; set; }

        public string ItemName { get; set; }

        public bool WithDRNumber { get; set; }

        public string Remarks { get; set; }

        public DeliveryTypeEnum?[] DeliveryType { get; set; }

        public string filter { get; set; }
    }

}
