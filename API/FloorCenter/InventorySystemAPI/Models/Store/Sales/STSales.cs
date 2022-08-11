using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Store.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Sales
{

    [Validator(typeof(STSalesValidator))]
    public class STSales : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? StoreId { get; set; }

        [Column(Order = 1)]
        [DisplayName("SI/PO Number")]
        public string SIPONumber { get; set; }

        [Column(Order = 2, TypeName = "date")]
        [DisplayName("SI/PO Date")]
        public DateTime? SIPODate { get; set; }

        [Column(Order = 3)]
        public string CustomerName { get; set; }

        [Column(Order = 4)]
        public string ContactNumber { get; set; }

        [Column(Order = 5)]
        public string CustomerAddress1 { get; set; }

        [Column(Order = 6)]
        public string CustomerAddress2 { get; set; }

        [Column(Order = 7)]
        public string CustomerAddress3 { get; set; }

        [Column(Order = 8)]
        public PaymentTypeEnum? PaymentType { get; set; }

        public string PaymentTypeStr
        {
            get
            {
                if (this.PaymentType.HasValue)
                {
                    return Enum.GetName(typeof(PaymentTypeEnum), this.PaymentType);
                }

                return null;
            }
        }

        [Column(Order = 9)]
        public DeliveryTypeEnum? DeliveryType { get; set; }

        public string DeliveryTypeStr
        {
            get
            {
                if (this.DeliveryType.HasValue)
                {
                    return Enum.GetName(typeof(DeliveryTypeEnum), this.DeliveryType);
                }

                return null;
            }
        }

        [Column(Order = 10)]
        [DisplayName("Sales Agent")]
        public int? Agent { get; set; }

        [Column(Order = 11)]
        public string Remarks { get; set; }

        public virtual ICollection<STSalesDetail> OrderedItems { get; set; }

    }
}
