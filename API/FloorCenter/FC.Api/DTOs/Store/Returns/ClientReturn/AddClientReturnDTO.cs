using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;

namespace FC.Api.DTOs.Store.Returns.ClientReturn
{
    public class AddClientReturnDTO
    {

        public int? StoreId { get; set; }

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int Id { get; set; }

        public ClientReturnTypeEnum? ClientReturnType { get; set; }

        public DateTime? PickupDate { get; set; }

        public string ReturnDRNumber { get; set; }

        public ICollection<AddClientReturnItems> PurchasedItems { get; set; }

        public string Remarks { get; set; }

        public bool? isTonalityAny { get; set; }
    }
}
