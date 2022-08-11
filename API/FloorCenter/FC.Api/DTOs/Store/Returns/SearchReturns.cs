using FC.Api.Validators.Store.Returns;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store.Returns
{
    [Validator(typeof(SearchReturnsValidator))]
    public class SearchReturns : BaseGetAll
    {

        public string TransactionNo { get; set; }

        public string ReturnFormNumber { get; set; }

        public string Code { get; set; }

        public int? StoreId { get;  set; }

        [Column(TypeName = "date")]
        public DateTime? RequestDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RequestDateTo { get; set; }

        public RequestStatusEnum?[] RequestStatus { get; set; }

        public string DRNumber { get;  set; }

        public ReturnTypeEnum? ReturnType { get; set; }


        public OrderStatusEnum?[] OrderStatus { get; set; }

        public bool mainList { get; set; }

        

    }
}
