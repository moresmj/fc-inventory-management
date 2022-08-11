using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{
    public class SearchReceiveItems
    {

        public int? StoreId { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public string TransactionNo { get; set; }

        public string PONumber { get; set; }

        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDateTo { get; set; }

    }
}
