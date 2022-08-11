using FC.Api.Validators.Store;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(AddSalesOrderItemsValidator))]
    public class AddSalesOrderItems
    {

        public int? StoreId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

    }
}
