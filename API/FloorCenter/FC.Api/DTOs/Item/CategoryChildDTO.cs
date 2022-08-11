using FC.Api.Validators.Item;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace FC.Api.DTOs.Item
{
    [Validator(typeof(CategoryChildDTOValidator))]
    public class CategoryChildDTO
    {

        public int Id { get; set; }

        public int? CategoryParentId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CategoryGrandChildDTO> GrandChildren { get; set; }

    }
}
