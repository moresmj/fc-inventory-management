using FC.Api.Validators.Store.BranchOrders;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store.BranchOrders
{
    public class UpdateTransferRequestDTO
    {
        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? Id { get; set; }

        public int? OrderToStoreId { get; set; }

        public string ORNumber { get; set; }

        public string SINumber { get; set; }

        public string WHDRNumber { get; set; }

        public string TransactionNo { get; set; }

    }
}
