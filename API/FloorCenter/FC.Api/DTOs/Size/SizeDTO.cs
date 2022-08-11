using FC.Api.Validators.Size;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Size
{

    [Validator(typeof(SizeDTOValidator))]
    public class SizeDTO
    {

        public int Id { get; set; }

        public string Name { get; set; }

    }
}
