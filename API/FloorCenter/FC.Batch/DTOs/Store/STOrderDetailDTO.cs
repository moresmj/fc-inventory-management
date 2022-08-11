using FC.Batch.DTOs.Item;
using FC.Batch.Helpers;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Batch.DTOs.Store
{
    public class STOrderDetailDTO
    {

        public int Id { get; set; }

        public int? STOrderId { get; set; }

        public int? ItemId { get; set; }

        public int? RequestedQuantity { get; set; }

        public string RequestedRemarks { get; set; }

        public int? ApprovedQuantity { get; set; }

        public string ApprovedRemarks { get; set; }

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

        public virtual ItemDTO Item { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateReleased { get; set; }

    }
}
