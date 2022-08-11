namespace FC.Api.DTOs.Store.Returns.ClientReturn
{
    public class SearchClientReturn : BaseGetAll
    {

        public int? StoreId { get; set; }

        /// <summary>
        /// STSales.ClientName
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// STSales.SINumber
        /// </summary>
        public string SINumber { get; set; }

        /// <summary>
        /// STSales.ORNumber
        /// </summary>
        public string ORNumber { get; set; }

    }
}
