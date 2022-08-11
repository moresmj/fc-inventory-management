namespace FC.Api.DTOs.Store.ReceiveItems
{
    public class ReceiveClientReturnsItems
    {

        /// <summary>
        /// STClientReturn.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// STClientReturn.STReturnId
        /// </summary>
        public int? STReturnId { get; set; }

        /// <summary>
        /// STClientReturn.ItemId
        /// </summary>
        public int? ItemId { get; set; }

        public string ReceivedRemarks { get; set; }

        public int? StoreId { get; set; }
    }
}
