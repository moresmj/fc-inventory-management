using FC.Api.Validators.Store;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(AddClientDeliveryItemsValidator))]
    public class AddClientDeliveryItems
    {

        public int? STOrderId { get; set; }

        public int? STOrderDetailId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public bool? isTonalityAny { get; set; }

    }
}
