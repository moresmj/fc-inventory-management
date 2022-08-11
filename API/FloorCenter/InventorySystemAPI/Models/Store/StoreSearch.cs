using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Store;

namespace InventorySystemAPI.Models.Store
{
    [Validator(typeof(StoreSearchValidator))]
    public class StoreSearch 
    {

        public int? Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public int? CompanyId { get; set; }

        public int? WarehouseId { get; set; }

    }
}
