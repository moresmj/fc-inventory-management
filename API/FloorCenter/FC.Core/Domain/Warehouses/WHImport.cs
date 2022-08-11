using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Warehouses
{
    public class WHImport : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        #endregion

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateUploaded { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateApproved { get; set; }

        public ICollection<WHImportDetail> Details { get; set; }

        public PhysicalCountTypeEnum PhysicalCountType { get; set; }

        #region Navigation Properties

        public Warehouse Warehouse { get; set; }

        #endregion

    }
}
