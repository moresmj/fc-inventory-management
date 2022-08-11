using FluentValidation.Attributes;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Validators.Store.Delivery;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Request
{

    [Validator(typeof(STDeliveryValidator))]
    public class STDelivery : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? StoreId { get; set; }

        [Column(Order = 2)]
        public int? STInventoryId { get; set; }

        [Column(Order = 3)]
        public DateTime? DeliveryDate { get; set; }

        [Column(Order = 4)]
        public int? ReceivedBy { get; set; }

        public virtual ICollection<STDeliveryDetail> ItemsToBeDelivered { get; set; }

        public virtual STInventory STInventory { get; set; }
        
    }
}
