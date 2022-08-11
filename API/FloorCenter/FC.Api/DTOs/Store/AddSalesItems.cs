using FC.Api.Validators.Store;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(AddSalesItemsValidator))]
    public class AddSalesItems
    {

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public int? StoreId { get; internal set; }
    }
}
