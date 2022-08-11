using FC.Batch.DTOs.Item;
using FC.Batch.Helpers;
using FC.Core.Domain.Common;
using System;

namespace FC.Batch.DTOs.Warehouse
{
    public class WHStockDTO
    {

        public int Id { get; set; }

        public int? WarehouseId { get; set; }

        public int? STOrderDetailId { get; set; }

        public int? WHReceiveDetailId { get; set; }

        public int? ItemId { get; set; }

        public int? OnHand { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public string TransactionTypeStr
        {
            get
            {
                if (this.TransactionType.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), this.TransactionType));
                }

                return null;
            }
        }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public string DeliveryStatusStr
        {
            get
            {
                if (this.DeliveryStatus.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), this.DeliveryStatus));
                }

                return null;
            }
        }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public string ReleaseStatusStr
        {
            get
            {
                if (this.ReleaseStatus.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), this.ReleaseStatus));
                }

                return null;
            }
        }

        public DateTime? DateReleased { get; set; }

        public ItemDTO Item { get; set; }

        public int? WHDeliveryDetailId { get; set; }

        public bool Broken { get; set; }

        /// <summary>
        /// STShowroomDelivery.Id
        /// </summary>
        public int? STShowroomDeliveryId { get; set; }

        /// <summary>
        /// STClientDelivery.Id
        /// </summary>
        public int? STClientDeliveryId { get; set; }

        //added for job execution
        public DateTime? ChangeDate { get; set; }
    }
}
