using FC.Batch.Helpers;
using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Batch.DTOs.Store
{
    public class STSalesDTO
    {

        public int Id { get; set; }

        public int? STOrderId { get; set; }

        public string TransactionNo { get; set; }

        public int? StoreId { get; set; }

        public int? OrderToStoreId { get; set; }

        public string SINumber { get; set; }

        public string ORNumber { get; set; }

        public string DRNumber { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Remarks { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public string SalesAgent { get; set; }

        public STOrderDTO Order { get; set; }

        public StoreDTO Store { get; set; }

        public ICollection<STSalesDetailDTO> SoldItems { get; set; }

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

        public SalesTypeEnum? SalesType { get; set; }

        public string SalesTypeStr
        {
            get
            {
                if (this.SalesType.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), this.SalesType));
                }

                return null;
            }
        }

        [Column(TypeName = "date")]
        public DateTime? SalesDate { get; set; }

        public ICollection<STDeliveryDTO> Deliveries { get; set; }

    }
}
