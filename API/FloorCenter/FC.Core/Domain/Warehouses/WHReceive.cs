using FC.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Warehouses
{
    public class WHReceive : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        /// <summary>
        /// User.Id
        /// </summary>
        [DisplayName("Checked By")]
        public int? UserId { get; set; }

        #endregion

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        [MaxLength(50)]
        public string PONumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PODate { get; set; }

        [MaxLength(50)]
        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DRDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReceivedDate { get; set; }

        public string Remarks { get; set; }

        public ICollection<WHReceiveDetail> ReceivedItems { get; set; }

        #region Navigation Properties

        public User User { get; set; }

        #endregion

    }
}
