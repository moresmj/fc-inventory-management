namespace FC.Api.DTOs.Store
{
    public class SearchSalesForReleasing
    {

        /// <summary>
        /// Searched by STOrder.PONumber
        /// </summary>
        public string PONumber { get; set; }

        /// <summary>
        /// Searched by STSales.SINumber
        /// </summary>
        public string SINumber { get; set; }

        /// <summary>
        /// Searched by STSales.ORNumber
        /// </summary>
        public string ORNumber { get; set; }

        /// <summary>
        /// Searched by STSales.DRNumber
        /// </summary>
        public string DRNumber { get; set; }

        /// <summary>
        /// Searched by STSales.ClientName
        /// </summary>
        public string ClientName { get; set; }

        public int? StoreId { get; internal set; }

    }
}
