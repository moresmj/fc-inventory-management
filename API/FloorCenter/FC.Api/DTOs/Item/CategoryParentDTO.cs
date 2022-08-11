using FC.Api.Validators.Item;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace FC.Api.DTOs.Item
{
    [Validator(typeof(CategoryParentDTOValidator))]
    public class CategoryParentDTO
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CategoryChildDTO> Children { get; set; }

    }
}
