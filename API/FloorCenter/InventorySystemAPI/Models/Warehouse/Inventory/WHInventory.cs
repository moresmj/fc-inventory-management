using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Warehouse.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Warehouse.Inventory
{

    [Validator(typeof(WHInventoryValidator))]
    public class WHInventory : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string TransactionNo { get; set; }

        [Column(Order = 2)]
        public int? WarehouseId { get; set; }

        [Column(Order = 3)]
        public string PONumber { get; set; }

        [Column(Order = 4, TypeName = "date")]
        public DateTime? PODate { get; set; }

        [Column(Order = 5)]
        public string DRNumber { get; set; }

        [Column(Order = 6, TypeName = "date")]
        public DateTime? DRDate { get; set; }

        [Column(Order = 7)]
        public DateTime? ReceivedDate { get; set; }

        [Column(Order = 8)]
        public string Remarks { get; set; }

        [Column(Order = 9)]
        public int? CheckedBy { get; set; }

        public virtual ICollection<WHInventoryDetail> DeliveredItems { get; set; }

    }
}
