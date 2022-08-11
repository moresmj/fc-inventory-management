using FC.Api.Validators.Warehouse;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Warehouse
{
    [Validator(typeof(WHReceiveSearchDTOValidator))]
    public class WHReceiveSearchDTO : BaseGetAll
    {

        public int? WarehouseId { get; set; }

        public string PONumber { get; set; }

        public string DRNumber { get; set; }

        public string ItemName { get; set; }

        public int? UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODateTo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DRDateFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DRDateTo { get; set; }






    }
}
