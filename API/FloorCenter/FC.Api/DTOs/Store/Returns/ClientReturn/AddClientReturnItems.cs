using FC.Core.Domain.Common;
using System.ComponentModel;

namespace FC.Api.DTOs.Store.Returns.ClientReturn
{
    public class AddClientReturnItems
    {

        /// <summary>
        /// STSalesDetail.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// STSalesDetail.STSalesId
        /// </summary>
        public int? STSalesId { get; set; }

        /// <summary>
        /// STSalesDetail.ItemId
        /// </summary>
        public int? ItemId { get; set; }

        [DisplayName("Return Qty")]
        public int? Quantity { get; set; }

        [DisplayName("Reason")]
        public ReturnReasonEnum? ReturnReason { get; set; }

        public string Remarks { get; set; }

    }
}
