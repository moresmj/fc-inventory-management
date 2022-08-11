using FC.Core.Domain.Common;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FC.Core.Domain.Stores
{
    public class STAdvanceOrder : BaseEntity
    {
        public int? StoreId { get; set; }

        public int? WarehouseId { get; set; }

        [MaxLength(50)]
        public string AONumber { get; set; }

        [MaxLength(50)]
        public string SINumber { get; set; }

        [MaxLength(50)]
        public string PONumber { get; set; }

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

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public OrderStatusEnum? OrderStatus { get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

        public ICollection<STAdvanceOrderDetails> AdvanceOrderDetails { get; set; }


        public DateTime? ApproveDate { get; set; }


        #region Navigation Properties

        public Store Store { get; set; }

        public Warehouse Warehouse { get; set; }

        #endregion

        public string changeStatusReasons { get; set; }
    }
}
