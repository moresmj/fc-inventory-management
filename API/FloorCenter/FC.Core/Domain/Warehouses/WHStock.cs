using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Stores;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Warehouses
{
    public class WHStock : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        /// <summary>
        /// WHReceiveDetail.Id
        /// </summary>
        public int? WHReceiveDetailId { get; set; }

        /// <summary>
        /// STOrderDetail.Id
        /// </summary>
        public int? STOrderDetailId { get; set; }

        /// <summary>
        /// WHDeliveryDetail.Id
        /// </summary>
        public int? WHDeliveryDetailId { get; set; }

        /// <summary>
        /// STShowroomDelivery.Id
        /// </summary>
        public int? STShowroomDeliveryId { get; set; }

        /// <summary>
        /// STClientDelivery.Id
        /// </summary>
        public int? STClientDeliveryId { get; set; }

        /// <summary>
        /// WHImportDetail.Id
        /// </summary>
        public int? WHImportDetailId { get; set; }

        /// <summary>
        /// WHImportDetail.Id
        /// </summary>
        public int? WHAllocateAdvanceOrderDetailId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? OnHand { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateReleased { get; set; }

        public bool Broken { get; set; }

        public int Available { get; set; }

        //added for advance order
        public int? Reserved { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        public STOrderDetail STOrderDetail { get; set; }

        public WHReceiveDetail WHReceiveDetail { get; set; }

        public WHDeliveryDetail WHDeliveryDetail { get; set; }

        public STShowroomDelivery STShowroomDelivery { get; set; }

        public STClientDelivery STClientDelivery { get; set; }

        public WHImportDetail WHImportDetail { get; set; }

        public WHAllocateAdvanceOrderDetail WHAllocateAdvanceOrderDetail { get; set; }

        //added for job execution
        public DateTime? ChangeDate { get; set; }

        #endregion

    }
}
