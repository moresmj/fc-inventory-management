using FluentValidation.Attributes;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Validators.Store.Releasing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Releasing
{

    [Validator(typeof(STReleaseValidator))]
    public class STRelease : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        [Column(Order = 1, TypeName = "date")]
        public DateTime? DateReleased { get; set; }

        [DisplayName("Delivery Number")]
        public string DRNo { get; set; }

        [Column(Order = 3, TypeName = "date")]
        public DateTime? DeliveryDate { get; set; }

        public virtual ICollection<STReleaseDetail> ReleasedItems { get; set; }

    }
}
