using FC.Api.Validators.Company;
using FluentValidation.Attributes;
using System;
using System.ComponentModel;

namespace FC.Api.DTOs.Company
{
    [Validator(typeof(CompanyDTOValidator))]
    public class CompanyDTO
    {

        public int Id { get; set; }

        [DisplayName("Company Code")]
        public string Code { get; set; }

        [DisplayName("Company Name")]
        public string Name { get; set; }


        public DateTime? DateCreated { get; set; }


    }
}
