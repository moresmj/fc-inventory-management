using InventorySystemAPI.Models.Warehouse.Stock;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Inventory
{
    public class STInventoryDetail : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? STInventoryId { get; set; }

        [Column(Order = 2)]
        public int? ItemId { get; set; }

        [Column(Order = 3)]
        public int? RequestedQuantity { get; set; }

        [Column(Order = 4)]
        public string RequestedRemarks { get; set; }

        [Column(Order = 5)]
        public int? ApprovedQuantity { get; set; }

        [Column(Order = 6)]
        public string ApprovedRemarks { get; set; }

        [Column(Order = 7)]
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
