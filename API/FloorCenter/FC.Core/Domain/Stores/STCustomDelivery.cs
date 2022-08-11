using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FC.Core.Domain.Stores
{
    public class STCustomDelivery : BaseEntity
    {
        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? STOrderId { get; set; }

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? STSalesId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? DeliveryFromStoreId { get; set; }

        #endregion

        [MaxLength(50)]
        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ApprovedDeliveryDate { get; set; }

        [MaxLength(255)]
        public string DriverName { get; set; }

        [MaxLength(10)]
        public string PlateNumber { get; set; }

        [MaxLength(50)]
        public string SINumber { get; set; }

        [MaxLength(50)]
        public string ORNumber { get; set; }

        public PreferredTimeEnum? PreferredTime { get; set; }

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

        //flag for partial receiving
        public bool IsRemainingForReceivingDelivery { get; set; }

        public ICollection<STShowroomDelivery> ShowroomDeliveries { get; set; }

        public ICollection<STClientDelivery> ClientDeliveries { get; set; }

        //public STOrder Order { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public DeliveryStatusEnum? Delivered { get; set; }

        #region Navigation Properties

        public Store Store { get; set; }

        #endregion
    }
}
