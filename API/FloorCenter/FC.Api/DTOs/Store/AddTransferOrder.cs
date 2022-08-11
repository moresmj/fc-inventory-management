using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store
{
    public class AddTransferOrder
    {

        public int? StoreId { get; set; }
        public ICollection<AddTransferOrderItems> TransferredItems { get; set; }
    }
}
