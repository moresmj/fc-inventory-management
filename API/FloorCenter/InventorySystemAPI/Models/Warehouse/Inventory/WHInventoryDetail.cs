using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Warehouse.Inventory;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Warehouse.Inventory
{

    [Validator(typeof(WHInventoryDetailValidator))]
    public class WHInventoryDetail : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? WHInventoryId { get; set; }

        [Column(Order = 2)]
        public int? ItemId { get; set; }

        [Column(Order = 3)]
        public int? Quantity { get; set; }

        [Column(Order = 4)]
        public string Remarks { get; set; }

    }
}
