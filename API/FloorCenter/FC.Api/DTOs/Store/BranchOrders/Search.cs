using FC.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store.BranchOrders
{
    public class Search : BaseGetAll
    {
        public int? OrderedBy { get; set; }

        public int? OrderToStoreId { get; set; }

        public string PONumber { get; set; }

        public string TransactionNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateTo { get; set; }

        public RequestStatusEnum?[] RequestStatus { get; set; }

        public StoreCompanyRelationEnum?[] StoreCompanyRelation { get; set; }

    }
}
