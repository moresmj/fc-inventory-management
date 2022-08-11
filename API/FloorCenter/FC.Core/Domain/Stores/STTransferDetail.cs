namespace FC.Core.Domain.Stores
{
    public class STTransferDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STTransfer.Id
        /// </summary>
        public int? STTransferId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? Quantity { get; set; }

    }
}
