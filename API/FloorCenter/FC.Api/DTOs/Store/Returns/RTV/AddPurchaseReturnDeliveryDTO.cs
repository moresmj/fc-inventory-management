using FC.Api.Validators.Warehouse;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;

namespace FC.Api.DTOs.Store.Returns
{

    [Validator(typeof(AddPurchaseReturnDeliveryDTOValidator))]
    public class AddPurchaseReturnDeliveryDTO
    {

        /// <summary>
        /// STReturn.Id
        /// </summary>
        public int Id { get; set; }

        public int? StoreId { get; set; }

        public string DRNumber { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string Remarks { get; set; }

        public ICollection<AddPurchaseReturnDeliveryItems> WarehouseDeliveries { get; set; }

    }
}
