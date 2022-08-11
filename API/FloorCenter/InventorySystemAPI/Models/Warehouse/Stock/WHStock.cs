using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Warehouse.Stock
{
    public class WHStock : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? WHInventoryDetailId { get; set; }

        [Column(Order = 2)]
        public int? STDeliveryDetailId { get; set; }

        [Column(Order = 3)]
        public int? ItemId { get; set; }

        [Column(Order = 4)]
        public int? OnHand { get; set; }

        [Column(Order = 5)]
        public TransactionTypeEnum? TransactionType { get; set; }

        public string TransactionTypeStr
        {
            get
            {
                if (this.TransactionType.HasValue)
                {
                    return Enum.GetName(typeof(TransactionTypeEnum), this.TransactionType);
                }

                return null;
            }
        }

        [Column(Order = 6)]
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

    }
}
