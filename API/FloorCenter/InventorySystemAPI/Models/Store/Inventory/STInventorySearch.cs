using FluentValidation.Attributes;
using InventorySystemAPI.Models.Warehouse.Stock;
using InventorySystemAPI.Validators.Store.Inventory;
using System;

namespace InventorySystemAPI.Models.Store.Inventory
{

    [Validator(typeof(STInventorySearchValidator))]
    public class STInventorySearch
    {

        public int? Id { get; set; }

        public int? StoreId { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public int? WarehouseId { get; set; }

        public string PONumber { get; set; }

        public DateTime? PODate { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

    }
}
