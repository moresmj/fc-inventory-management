using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Stores
{
    public class STImport : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        #endregion

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateUploaded { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateApproved { get; set; }

        public ICollection<STImportDetail> Details { get; set; }

        #region Navigation Properties

        //public STImportDetail ImportDetails { get; set; }

        public Store Store { get; set; }

        public PhysicalCountTypeEnum PhysicalCountType { get; set; }

        #endregion

    }
}
