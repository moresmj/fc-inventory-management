using System.Collections.Generic;

namespace FC.Core.Domain.Stores
{
    public class STTransfer : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        #endregion

        public ICollection<STTransferDetail> TransferredItems { get; set; }

    }
}
