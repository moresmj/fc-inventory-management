using FC.Core.Domain.Common;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Stores
{
    public class STReturn : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? STSalesId { get; set; }

        #endregion

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        [MaxLength(50)]
        public string ReturnFormNumber { get; set; }

        public string Remarks { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public ReturnTypeEnum? ReturnType { get; set; }

        public ClientReturnTypeEnum? ClientReturnType { get; set; }

        //added for optimization and filtering
        public OrderStatusEnum? OrderStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PickupDate { get; set; }

        [MaxLength(50)]
        public string ReturnDRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ApprovedDeliveryDate { get; set; }

        [MaxLength(255)]
        public string DriverName { get; set; }

        [MaxLength(10)]
        public string PlateNumber { get; set; }

        public ICollection<STPurchaseReturn> PurchasedItems { get; set; }

        public ICollection<STClientReturn> ClientPurchasedItems { get; set; }

        public ICollection<WHDelivery> Deliveries { get; set; }

        #region Navigation Properties

        public Store Store { get; set; }

        public Warehouse Warehouse { get; set; }

        #endregion

    }
}
