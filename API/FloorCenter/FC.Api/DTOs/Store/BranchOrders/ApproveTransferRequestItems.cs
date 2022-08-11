namespace FC.Api.DTOs.Store.BranchOrders
{
    public class ApproveTransferRequestItems
    {

        /// <summary>
        /// STOrderDetail.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? STOrderId { get; set; }

        public int? StoreId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        public int? ApprovedQuantity { get; set; }

        public string ApprovedRemarks { get; set; }

    }
}
