using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Stores
{
    public class STSales : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? STOrderId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? OrderToStoreId { get; set; }

        /// <summary>
        /// StDelivery.Id
        /// </summary>
        public int? STDeliveryId { get; set; }

        #endregion

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        [MaxLength(50)]
        public string SINumber { get; set; }

        [MaxLength(50)]
        public string ORNumber { get; set; }

        [MaxLength(50)]
        public string DRNumber { get; set; }

        [MaxLength(255)]
        public string ClientName { get; set; }

        [MaxLength(50)]
        public string ContactNumber { get; set; }

        [MaxLength(255)]
        public string Address1 { get; set; }

        [MaxLength(255)]
        public string Address2 { get; set; }

        [MaxLength(255)]
        public string Address3 { get; set; }

        public string Remarks { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        [MaxLength(255)]
        public string SalesAgent { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        public SalesTypeEnum? SalesType { get; set; }

        public OrderStatusEnum? OrderStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SalesDate { get; set; }

        public ICollection<STSalesDetail> SoldItems { get; set; }

        public ICollection<STDelivery> Deliveries { get; set; }

        #region Navigation Properties

        public STOrder Order { get; set; }

        #endregion

    }
}
