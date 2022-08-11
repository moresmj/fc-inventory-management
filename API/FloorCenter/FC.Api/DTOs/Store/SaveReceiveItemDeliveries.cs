using FC.Api.Validators.Store;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(SaveReceiveItemDeliveriesValidator))]
    public class SaveReceiveItemDeliveries
    {

        /// <summary>
        /// STShowroomDelivery.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// STDelivery.Id
        /// </summary>
        public int? STDeliveryId { get; set; }

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? STOrderId { get; set; }

        /// <summary>
        /// STOrderDetail.Id
        /// </summary>
        public int? STOrderDetailId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        public int? DeliveredQuantity { get; set; }

        public string Remarks { get; set; }

        //flag for partial receiving
        public bool IsRemainingForReceiving { get; set; }

    }
}
