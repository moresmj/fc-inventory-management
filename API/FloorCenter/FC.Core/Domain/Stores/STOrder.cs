using FC.Core.Domain.Common;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Stores
{
    public class STOrder : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? OrderToStoreId { get; set; }

        /// <summary>
        /// STTransfer.Id
        /// </summary>
        public int? STTransferId { get; set; }

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        // added client request to see remaining qty to be allocated
        public int? STAdvanceOrderId { get; set; }

        #endregion

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        public OrderTypeEnum? OrderType { get; set; }

        [MaxLength(50)]
        public string PONumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODate { get; set; }

        [MaxLength(255)]
        public string SalesAgent { get; set; }

        [MaxLength(255)]
        public string ClientName { get; set; }

        [MaxLength(255)]
        public string ContactNumber { get; set; }

        [MaxLength(255)]
        public string Address1 { get; set; }

        [MaxLength(255)]
        public string Address2 { get; set; }

        [MaxLength(255)]
        public string Address3 { get; set; }

        public string Remarks { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }


        //added for optimization purposes
        public OrderStatusEnum? OrderStatus { get; set; }

        [MaxLength(50)]
        public string ORNumber { get; set; }

        [MaxLength(50)]
        public string SINumber { get; set; }

        [MaxLength(50)]
        public string ClientSINumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        [MaxLength(50)]
        public string WHDRNumber { get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

        public string TRNumber { get; set; }

        public ICollection<STOrderDetail> OrderedItems { get; set; }

        public ICollection<STDelivery> Deliveries { get; set; }

        //Added for splitting
        public bool? isDealerOrder { get; set; }

        public bool? isAdvanceOrderFlg { get; set; }

        #region Navigation Properties

        public Store Store { get; set; }

        public Warehouse Warehouse { get; set; }

        #endregion

    }
}
