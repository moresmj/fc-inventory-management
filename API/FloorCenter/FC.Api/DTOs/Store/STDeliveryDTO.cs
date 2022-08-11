using FC.Api.Validators.Store;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(STDeliveryDTOValidator))]
    public class STDeliveryDTO
    {

        public int Id { get; set; }

        public int? StoreId { get; set; }

        public int? STOrderId { get; set; }

        public int? DeliveryFromStoreId { get; set; }

        public string DRNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ApprovedDeliveryDate { get; set; }

        public string DriverName { get; set; }

        public string PlateNumber { get; set; }

        public string SINumber { get; set; }

        public string ORNumber { get; set; }

        public PreferredTimeEnum? PreferredTime { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Remarks { get; set; }

        public ICollection<STShowroomDeliveryDTO> ShowroomDeliveries { get; set; }

        public ICollection<STClientDeliveryDTO> ClientDeliveries { get; set; }

        public STOrderDTO Order { get; set; }

        public StoreDTO Store { get; set; }

        public STSalesDTO Sales { get; set; }

        public int? STSalesId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public DeliveryStatusEnum? Delivered { get; set; }

        public bool? IsClienOrder { get
            {
                        if (this.Order != null)
                        {
                            if(this.Order.DeliveryType == DeliveryTypeEnum.Pickup && this.Order.OrderType == OrderTypeEnum.ClientOrder)
                            {
                                return true;
                            }
                            return false;
                        
                        }

                        return false;
             }

        }

    }
}
