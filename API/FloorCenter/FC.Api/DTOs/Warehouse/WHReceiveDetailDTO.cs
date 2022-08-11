using FC.Api.DTOs.Item;
using FC.Api.Validators.Warehouse;
using FC.Api.Validators.Warehouse.ModifyTonality;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Warehouse
{

    [Validator(typeof(WHModifyItemTonalityDetailsDTOValidator))]
    public class WHReceiveDetailDTO
    {

        public int Id { get; set; }

        public int? WHReceiveId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public int? ReservedQuantity { get; set; }

        public string Remarks { get; set; }

        public ItemDTO Item { get; set; }

    }
}
