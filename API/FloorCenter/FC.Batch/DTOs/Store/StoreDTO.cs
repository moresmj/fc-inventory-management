
using FC.Batch.DTOs.Warehouse;
using FluentValidation.Attributes;
using System;
using System.ComponentModel;

namespace FC.Batch.DTOs.Store
{
    public class StoreDTO
    {

        public int Id { get; set; }

        [DisplayName("Store Code")]
        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public int? CompanyId { get; set; }

        public int? WarehouseId { get; set; }

        public virtual WarehouseDTO Warehouse { get; set; }

        public DateTime? DateCreated { get; set; }

    }
}
