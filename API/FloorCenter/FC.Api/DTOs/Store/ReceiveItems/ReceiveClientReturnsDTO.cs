using System.Collections.Generic;

namespace FC.Api.DTOs.Store.ReceiveItems
{
    public class ReceiveClientReturnsDTO
    {

        /// <summary>
        /// STReturn.Id
        /// </summary>
        public int Id { get; set; }

        public int? StoreId { get; set; }

        public string TransactionNo { get; set; }

        public ICollection<ReceiveClientReturnsItems> ClientPurchasedItems { get; set; }

    }
}
