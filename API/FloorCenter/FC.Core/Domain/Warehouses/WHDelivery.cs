using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Warehouses
{
    public class WHDelivery : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// STReturn.Id
        /// </summary>
        public int? STReturnId { get; set; }

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

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public ICollection<WHDeliveryDetail> WarehouseDeliveries { get; set; }


        public string Remarks { get; set; }

        #region Navigation Properties

        public Store Store { get; set; }

        #endregion

    }
}
