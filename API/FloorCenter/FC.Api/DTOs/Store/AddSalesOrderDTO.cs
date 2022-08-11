using FC.Api.Validators.Store;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{

    public class AddSalesOrderDTO
    {

        public int? StoreId { get; set; }

        public string SINumber { get; set; }

        public string ORNumber { get; set; }

        public string DRNumber { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SalesDate { get; set; }

        public string SalesAgent { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public ICollection<AddSalesOrderItems> SoldItems { get; set; }

    }

}
