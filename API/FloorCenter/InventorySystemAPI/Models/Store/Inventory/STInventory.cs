using FluentValidation.Attributes;
using InventorySystemAPI.Models.Warehouse.Stock;
using InventorySystemAPI.Validators.Store.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Inventory
{

    [Validator(typeof(STInventoryValidator))]
    public class STInventory : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string TransactionNo { get; set; }

        [Column(Order = 2)]
        public int? StoreId { get; set; }

        [Column(Order = 3)]
        public TransactionTypeEnum? TransactionType { get; set; }

        public string TransactionTypeStr
        {
            get
            {
                if (this.TransactionType.HasValue)
                {
                    return Enum.GetName(typeof(TransactionTypeEnum), this.TransactionType);
                }

                return null;
            }
        }

        [Column(Order = 4)]
        public int? WarehouseId { get; set; }

        [Column(Order = 5)]
        public string PONumber { get; set; }

        [Column(Order = 6, TypeName = "date")]
        public DateTime? PODate { get; set; }

        [Column(Order = 7)]
        public string Remarks { get; set; }

        [Column(Order = 8)]
        public RequestStatusEnum? RequestStatus { get; set; }

        public string RequestStatusStr
        {
            get
            {
                if (this.RequestStatus.HasValue)
                {
                    return Enum.GetName(typeof(RequestStatusEnum), this.RequestStatus);
                }

                return null;
            }
        }

        public virtual ICollection<STInventoryDetail> RequestedItems { get; set; }

        public virtual Models.Warehouse.Warehouse Warehouse { get; set; }

    }
}
