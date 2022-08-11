using FC.Core;
using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store
{
    public class AddTransferOrderStoreDTO : BaseEntity
    {

        public int? StoreId { get; set; }

        public ICollection<AddTransferOrderStoreItemsDTO> TransferredItems { get; set; }

    }
}
