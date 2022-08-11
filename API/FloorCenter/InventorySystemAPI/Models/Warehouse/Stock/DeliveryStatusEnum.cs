namespace InventorySystemAPI.Models.Warehouse.Stock
{

    /// <summary>
    /// Represents the delivery status
    /// </summary>
    public enum DeliveryStatusEnum
    {

        Delivered = 20,

        /// <summary>
        /// No delivery date yet
        /// </summary>
        Pending = 21,

        /// <summary>
        /// Waiting delivery schedule
        /// </summary>
        Waiting = 22,

        NotDelivered = 23

    }


}
