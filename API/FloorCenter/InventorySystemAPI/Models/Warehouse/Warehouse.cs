using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Warehouse;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Warehouse
{

    [Validator(typeof(WarehouseValidator))]
    public class Warehouse : BaseEntity
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

    }
}
