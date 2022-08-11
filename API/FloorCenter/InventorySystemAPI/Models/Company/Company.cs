using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Company;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Company
{

    [Validator(typeof(CompanyValidator))]
    public class Company : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string Code { get; set; }

        [Column(Order = 2)]
        public string Name { get; set; }

    }
}
