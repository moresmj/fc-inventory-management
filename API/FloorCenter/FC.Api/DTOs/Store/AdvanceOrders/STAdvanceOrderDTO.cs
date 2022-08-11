using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Validators.Store.AdvanceOrders;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store.AdvanceOrder
{
    [Validator(typeof(STAdvanceOrderDTOValidator))]
    public class STAdvanceOrderDTO
    {

        public int Id { get; set; }

        public int? WarehouseId { get; set; }

        public int? StoreId { get; set; }

        public string AONumber { get; set; }

        public string SINumber { get; set; }

        public string PONumber { get; set;  }

        public string SalesAgent { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }
    
        public string Address3 { get; set; }

        public string Remarks { get; set; }

        public bool forUpdate { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public OrderStatusEnum? OrderStatus { get; set; }

        public string RequestStatusStr
        { get
            {
                if(this.RequestStatus.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), this.RequestStatus));
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

        public ICollection<STAdvanceOrderDetailsDTO> AdvanceOrderDetails { get; set; }


        public StoreDTO Store { get; set; }

        public WarehouseDTO Warehouse { get; set; }

        public string ChangeStatusReason { get; set; }

    }
}
