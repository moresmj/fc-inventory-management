namespace FC.Api.DTOs.Store.Returns
{
    public class AddPurchaseReturnDeliveryItems
    {

        /// <summary>
        /// STReturn.Id
        /// </summary>
        public int? STReturnId { get; set; }

        /// <summary>
        /// STPurchaseReturn.Id
        /// </summary>
        public int? STPurchaseReturnId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

    }
}
