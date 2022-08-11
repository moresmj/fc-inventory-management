using InventorySystemAPI.Models.Store.Request;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Stock
{
    public class STStock : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? STDeliveryDetailId { get; set; }

        [Column(Order = 2)]
        public int? STSalesDetailId { get; set; }

        [Column(Order = 3)]
        public int? ItemId { get; set; }

        [Column(Order = 4)]
        public int? OnHand { get; set; }

        public virtual STDeliveryDetail STDeliveryDetail { get; set; }

        public virtual Item.Item Item { get; set; }


    }
}
