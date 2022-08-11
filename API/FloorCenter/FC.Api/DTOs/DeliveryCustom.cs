using FC.Api.DTOs.Store;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs
{
    public class DeliveryCustom
    {
        public int Id { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? STOrderId { get; set; }

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? STSalesId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? DeliveryFromStoreId { get; set; }


        public string DRNumber { get; set; }


        public DateTime? DeliveryDate { get; set; }


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

        //flag for partial receiving
        public bool IsRemainingForReceivingDelivery { get; set; }

        public ICollection<STShowroomDelivery> ShowroomDeliveries { get; set; }

        public ICollection<STClientDelivery> ClientDeliveries { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public DeliveryStatusEnum? Delivered { get; set; }

        #region Navigation Properties

        public short Store { get; set; }

        public STOrderCustomDTO Details { get; set; }
        #endregion
    }


}
