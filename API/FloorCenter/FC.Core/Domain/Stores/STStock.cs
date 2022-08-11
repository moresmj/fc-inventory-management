using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Warehouses;
using System;

namespace FC.Core.Domain.Stores
{
    public class STStock : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// STShowroomDelivery.Id
        /// </summary>
        public int? STShowroomDeliveryId { get; set; }

        /// <summary>
        /// STOrderDetail.Id
        /// </summary>
        public int? STOrderDetailId { get; set; }

        /// <summary>
        /// STSalesDetail.Id
        /// </summary>
        public int? STSalesDetailId { get; set; }

        /// <summary>
        /// STClientDelivery.Id
        /// </summary>
        public int? STClientDeliveryId { get; set; }

        /// <summary>
        /// WHDeliveryDetail.Id
        /// </summary>
        public int? WHDeliveryDetailId { get; set; }

        /// <summary>
        /// STClientReturn.Id
        /// </summary>
        public int? STClientReturnId { get; set; }

        /// <summary>
        /// STImportDetail.Id
        /// </summary>
        public int? STImportDetailId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        public DateTime? ChangeDate { get; set; }

        #endregion

        public int? OnHand { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public bool Broken { get; set; }

        public int Available { get; set; }

        //adding flag to reflect on invetory history if interbranch delivery
        public bool IsDeliveryTransfer { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        public Store Store { get; set; }

        public STShowroomDelivery STShowroomDelivery { get; set; }

        public STSalesDetail STSalesDetail { get; set; }

        public STOrderDetail STOrderDetail { get; set; }

        public STClientDelivery STClientDelivery { get; set; }

        public STClientReturn STClientReturn { get; set; }

        public WHDeliveryDetail WHDeliveryDetail { get; set; }

        public STImportDetail STImportDetail { get; set; }

        #endregion

    }
}
