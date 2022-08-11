using FC.Api.Validators.Store.BranchOrders;
using FluentValidation.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace FC.Api.DTOs.Store.BranchOrders
{
    [Validator(typeof(ApproveTransferRequestDTOValidator))]
    public class ApproveTransferRequestDTO
    {

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? Id { get; set; }

        public string TransactionNo { get; set; }

        public ICollection<ApproveTransferRequestItems> TransferredItems { get; set; }
        
    }
}
