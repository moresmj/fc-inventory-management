using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store
{
    public class AddTransferOrderStoreItemsDTO : STTransferDetail
    {

        public int? StoreId { get; set; }

    }
}
