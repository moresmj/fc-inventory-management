using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{
    public class SearchApproveRequests : BaseGetAll
    {

        public int? StoreId { get; set; }

        public string PONumber { get; set; }

        public string TransactionNo { get; set; }

        public string AONumber { get; set; }

        public int? WarehouseId { get; set; }

        public string SiNumber { get; set; }

        public string ClientName { get; set; }

        public string OrderedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateTo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestDateTo { get; set; }

        public string ItemName { get; set; }

        public string ItemCode { get; set; }

        public bool? isDealer { get; set; }

        public bool? isAllocatePage { get; set; }

        public RequestStatusEnum?[] RequestStatus { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

        public OrderStatusEnum?[] Orderstatus { get; set; }

    }
}
