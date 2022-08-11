using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Size;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Size
{

    [Validator(typeof(SizeValidator))]
    public class Size : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string Name { get; set; }

    }
}
