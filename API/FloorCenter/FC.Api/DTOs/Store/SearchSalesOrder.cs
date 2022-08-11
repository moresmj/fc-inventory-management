using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{
    public class SearchSalesOrder :BaseGetAll
    {

        public int? StoreId { get; set; }

        public string SINumber { get; set; }

        public string ORNumber { get; set; }

        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SalesDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SalesDateTo { get; set; }

        public string ClientName { get; set; }

        public OrderStatusEnum?[] OrderStatus {get;set;}

    }
}
