using FC.Api.Validators.Store;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;

namespace FC.Api.DTOs.Store
{
    [Validator(typeof(AddSalesOrderDeliveryDTOValidator))]
    public class AddSalesOrderDeliveryDTO
    {

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int Id { get; set; }

        public string SINumber { get; set; }

        public string ORNumber { get; set; }

        public string DRNumber { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public PreferredTimeEnum? PreferredTime { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Remarks { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        public ICollection<AddSalesOrderDeliveryItems> ClientDeliveries { get; set; }
        
    }
}
