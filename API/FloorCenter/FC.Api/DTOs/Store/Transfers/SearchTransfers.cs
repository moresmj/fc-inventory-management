using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store.Transfers
{
    public class SearchTransfers :BaseGetAll
    {
        public int? StoreId { get; set; }

        public int? OrderToStoreId { get; set; }

        public string PONumber { get; set; }

        public string TransactionNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateTo { get; set; }

        public RequestStatusEnum?[] RequestStatus { get; set; }

        public StoreCompanyRelationEnum?[] StoreCompanyRelation { get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

    }
}
