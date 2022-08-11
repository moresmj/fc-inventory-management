namespace FC.Api.DTOs.Warehouse.Receive_Items
{
    public class ReceiveReturnsItems
    {

        /// <summary>
        /// WHDeliveryDetail.Id
        /// </summary>
        public int? Id { get; set; }

        public int? ItemId { get; set; }

        /// <summary>
        /// WHDelivery.Id
        /// </summary>
        public int? WHDeliveryId { get; set; }

        /// <summary>
        /// STPurchaseReturn.Id
        /// </summary>
        public int? STPurchaseReturnId { get; set; }

        public int? GoodQuantity { get; set; }

        public int? BrokenQuantity { get; set; }

        public string ReceivedRemarks { get; set; }

    }
}
