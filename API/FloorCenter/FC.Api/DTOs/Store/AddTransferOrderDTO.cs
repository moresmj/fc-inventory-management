using FC.Api.Validators.Store;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{

    public class AddTransferOrderDTO
    {

        public string PONumber { get; set; }

        [DisplayName("Order Date")]
        [Column(TypeName = "date")]
        public DateTime? PODate { get; set; }

        [DisplayName("Payment Mode")]
        public PaymentModeEnum? PaymentMode { get; set; }

        [DisplayName("Delivery Mode")]
        public DeliveryTypeEnum? DeliveryType { get; set; }

        public string SalesAgent { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Remarks { get; set; }

        public ICollection<AddTransferOrderStoreDTO> TransferOrders { get; set; }

        public int? TransferId { get; set; }

        [DisplayName("Order To")]
        public int? OrderToStoreId { get; set; }

        public int? StoreId { get; set; }

        public string TRNumber { get; set; }

        public bool? isDealer { get; set; }

    }
}
