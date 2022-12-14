using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Store;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store
{
    [Validator(typeof(StoreValidator))]
    public class Store : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string Code { get; set; }

        [Column(Order = 2)]
        public string Name { get; set; }

        [Column(Order = 3)]
        public string Address { get; set; }

        [Column(Order = 4)]
        public string ContactNumber { get; set; }

        [Column(Order = 5)]
        public int? CompanyId { get; set; }

        [Column(Order = 6)]
        public int? WarehouseId { get; set; }

        public virtual Models.Company.Company Company { get; set; }

        public virtual Models.Warehouse.Warehouse Warehouse { get; set; }

    }
}
