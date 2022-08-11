namespace FC.Api.DTOs.Store.Releasing
{
    public class ReleaseForClientOrderDTO
    {

        public int? StoreId { get; set; }

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? Id { get; set; }

        public string SINumber { get; set; }

        public string ORNumber { get; set; }

        public string DRNumber { get; set; }

        public string Remarks { get; set; }

        public string SalesAgent { get; set; }

        public string TransactionNo { get; set; }

    }
}
