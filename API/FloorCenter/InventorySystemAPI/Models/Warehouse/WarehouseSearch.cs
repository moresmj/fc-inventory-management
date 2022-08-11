using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Warehouse;

namespace InventorySystemAPI.Models.Warehouse
{

    [Validator(typeof(WarehouseSearchValidator))]
    public class WarehouseSearch
    {

        public int? Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

    }
}
