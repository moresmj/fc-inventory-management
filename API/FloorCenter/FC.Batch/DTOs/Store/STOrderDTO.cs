using FC.Batch.DTOs.Warehouse;
using FC.Batch.Helpers;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Batch.DTOs.Store
{
    public class STOrderDTO
    {

        public int Id { get; set; }

        public string TransactionNo { get; set; }

        public int? StoreId { get; set; }

        public int? OrderToStoreId { get; set; }

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

        [DisplayName("Delivery Mode")]
        public DeliveryTypeEnum? DeliveryType { get; set; }

        public string DeliveryTypeStr
        {
            get
            {
                if (this.DeliveryType.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), this.DeliveryType));
                }

                return null;
            }
        }

        public OrderTypeEnum? OrderType { get; set; }

        public string OrderTypeStr
        {
            get
            {
                if (this.OrderType.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), this.OrderType));
                }

                return null;
            }
        }

        [DisplayName("Order To")]
        public int? WarehouseId { get; set; }

        public string PONumber { get; set; }

        [DisplayName("Order Date")]
        [Column(TypeName = "date")]
        public DateTime? PODate { get; set; }

        public string Remarks { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public string RequestStatusStr
        {
            get
            {
                if (this.RequestStatus.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), this.RequestStatus));
                }

                return null;
            }
        }

        public ICollection<STOrderDetailDTO> OrderedItems { get; set; }

        public StoreDTO Store { get; set; }

        public WarehouseDTO Warehouse { get; set; }

        public ICollection<STDeliveryDTO> Deliveries { get; set; }

        public string SalesAgent { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string ORNumber { get; set; }

        public string SINumber { get; set; }

        public int? STTransferId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public string WHDRNumber { get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

        public string PaymentModeStr
        {
            get
            {
                if (this.PaymentMode.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), this.PaymentMode));
                }

                return null;
            }
        }

    }
}
