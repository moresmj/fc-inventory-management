using FC.Api.Validators.Item;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.Item
{
    [Validator(typeof(CategoryGrandChildDTOValidator))]
    public class CategoryGrandChildDTO
    {

        public int Id { get; set; }

        public int? CategoryChildId { get; set; }

        public string Name { get; set; }

    }
}
