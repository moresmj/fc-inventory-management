using FluentValidation.Attributes;
using InventorySystemAPI.Models.Warehouse.Stock;
using InventorySystemAPI.Validators.Store.Delivery;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Request
{

    [Validator(typeof(STDeliveryDetailValidator))]
    public class STDeliveryDetail : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? STDeliveryId { get; set; }

        [Column(Order = 2)]
        public int? STInventoryDetailId { get; set; }

        [Column(Order = 3)]
        public int? ItemId { get; set; }

        [Column(Order = 5)]
        public int? Quantity { get; set; }

        [Column(Order = 6)]
        public int? DeliveredQuantity { get; set; }

        [Column(Order = 7)]
        public string Remarks { get; set; }

        [Column(Order = 8)]
        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public string DeliveryStatusStr
        {
            get
            {
                if (this.DeliveryStatus.HasValue)
                {
                    return Enum.GetName(typeof(DeliveryStatusEnum), this.DeliveryStatus);
                }

                return null;
            }
        }

        public virtual Item.Item Item { get; set; }

    }
}
