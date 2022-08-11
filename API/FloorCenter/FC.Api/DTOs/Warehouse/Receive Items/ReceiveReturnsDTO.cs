using FC.Api.Validators.Warehouse.Receive_Items;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace FC.Api.DTOs.Warehouse.Receive_Items
{

    [Validator(typeof(ReceiveReturnsDTOValidator))]
    public class ReceiveReturnsDTO
    {

        /// <summary>
        /// WHDelivery.Id
        /// </summary>
        public int Id { get; set; }

        public int? WarehouseId { get; set; }

        public string TransactionNo { get; set; }

        public ICollection<ReceiveReturnsItems> DeliveredItems { get; set; }

    }
}
