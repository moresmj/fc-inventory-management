using FC.Api.Validators.Store;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Store
{
    [Validator(typeof(UpdateForDeliveryDTOValidator))]
    public class UpdateForDeliveryDTO : STDeliveryDTO
    {
        public string TransactionNo { get; set; }
    }
}
