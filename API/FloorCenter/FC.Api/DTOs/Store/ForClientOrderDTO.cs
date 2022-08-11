using FC.Api.Validators.Store;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(ForClientOrderDTOValidator))]
    public class ForClientOrderDTO : STOrderDTO
    { }
}
