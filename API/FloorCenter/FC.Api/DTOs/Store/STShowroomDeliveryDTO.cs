using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FC.Api.Validators.Store;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(STShowroomDeliveryDTOValidator))]
    public class STShowroomDeliveryDTO
    {

        public int Id { get; set; }

        public int? STDeliveryId { get; set; }

        public int? STOrderDetailId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public int? DeliveredQuantity { get; set; }

        public string Remarks { get; set; }

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

        public ItemDTO Item { get; set; }

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

        public bool? isTonalityAny { get; set; }


    }
}
