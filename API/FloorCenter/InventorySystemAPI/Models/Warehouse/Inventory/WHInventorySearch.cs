using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Warehouse.Inventory;
using System;

namespace InventorySystemAPI.Models.Warehouse.Inventory
{

    [Validator(typeof(WHInventorySearchValidator))]
    public class WHInventorySearch
    {

        public int? Id { get; set; }

        public string TransactionId { get; set; }

        public int? WarehouseId { get; set; }

        public string PONumber { get; set; }

        public DateTime? PODate { get; set; }

        public string DRNumber { get; set; }

        public DateTime? DRDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public int? CheckedBy { get; set; }
        
    }
}
