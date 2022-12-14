using FC.Core;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Stores
{
    public class STOrderListDTO :BaseEntity
    {

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? OrderToStoreId { get; set; }

        /// <summary>
        /// STTransfer.Id
        /// </summary>
        public int? STTransferId { get; set; }

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }



        public string TransactionNo { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        public OrderTypeEnum? OrderType { get; set; }


        public string PONumber { get; set; }


        public DateTime? PODate { get; set; }


        public string SalesAgent { get; set; }


        public string ClientName { get; set; }


        public string ContactNumber { get; set; }


        public string Address1 { get; set; }


        public string Address2 { get; set; }


        public string Address3 { get; set; }

        public string Remarks { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }


        public string ORNumber { get; set; }


        public string SINumber { get; set; }


        public string ClientSINumber { get; set; }


        public DateTime? ReleaseDate { get; set; }

        public string WHDRNumber { get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

        public string TRNumber { get; set; }

        public ICollection<STOrderDetail> OrderedItems { get; set; }

        public ICollection<STDelivery> Deliveries { get; set; }


        public Store Store { get; set; }

        public Warehouse Warehouse { get; set; }

        public bool isTransfer { get; set; }

        public bool isInterBranch { get; set; }

        public string TransactionTypeStr { get; set; }

        public int?  orderToStoreCompanyId { get; set; }

        public int? storeCompany { get; set; }
    }
}

