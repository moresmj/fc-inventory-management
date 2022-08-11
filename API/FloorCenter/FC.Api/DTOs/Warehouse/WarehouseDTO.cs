using FC.Api.Validators.Warehouse;
using FluentValidation.Attributes;
using System;
using System.ComponentModel;

namespace FC.Api.DTOs.Warehouse
{

    [Validator(typeof(WarehouseDTOValidator))]
    public class WarehouseDTO
    {

        public int Id { get; set; }

        [DisplayName("Warehouse Code")]
        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public bool Vendor { get; set; }

        public DateTime? DateCreated { get; set; }

    }
}
