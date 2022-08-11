namespace FC.Api.DTOs.Store.Releasing
{
    public class SearchTransfer : BaseGetAll
    {

        /// <summary>
        /// Holds current user storeid (User.StoreId)
        /// </summary>
        public int? StoreId { get; internal set; }

        /// <summary>
        /// Searched by STOrder.PONumber
        /// </summary>
        public string PONumber { get; set; }

    }
}
