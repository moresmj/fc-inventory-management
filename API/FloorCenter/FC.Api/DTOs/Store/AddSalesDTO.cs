using FC.Api.Validators.Store;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(AddSalesDTOValidator))]
    public class AddSalesDTO
    {

        public string SINumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        public string SalesAgent { get; set; }

        public int? StoreId { get; set; }

        public ICollection<AddSalesItems> SoldItems { get; set; }

        public bool IsSameDaySales { get; set; }

        //Add as per sir Bert to remove update on sds-releasing
        public string orNumber { get; set; }

        public DateTime? dateUpdated { get; set; }

        public string drNumber { get; set; }

        public string remarks { get; set; }

        public string ClientName { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string address3 { get; set; }

        public string contactNumber { get; set; }


        public string deliveryType { get; set; }

        public string transactionNo { get; set; }

        public string releaseItems { get; set; }
  

    }
}
