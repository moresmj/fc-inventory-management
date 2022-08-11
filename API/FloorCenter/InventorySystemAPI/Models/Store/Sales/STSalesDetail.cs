using FluentValidation.Attributes;
using InventorySystemAPI.Models.Store.Releasing;
using InventorySystemAPI.Models.Warehouse.Stock;
using InventorySystemAPI.Validators.Store.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Sales
{

    [Validator(typeof(STSalesDetailValidator))]
    public class STSalesDetail : BaseEntity
    {
        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? STSalesId { get; set; }

        [Column(Order = 2)]
        public int? ItemId { get; set; }

        [Column(Order = 3)]
        public int? Quantity { get; set; }

        [Column(Order = 4)]
        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public string DeliveryStatusStr
        {
            get
            {
                if (this.DeliveryStatus.HasValue)
                {
                    return Enum.GetName(typeof(DeliveryStatusEnum), this.DeliveryStatus);
                }

                return null;
            }
        }

        public virtual ICollection<STReleaseDetail> ReleasedItems { get; set; }

        public virtual STSales STSales { get; set; }

    }
}
