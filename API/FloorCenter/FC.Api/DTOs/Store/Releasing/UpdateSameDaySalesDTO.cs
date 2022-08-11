namespace FC.Api.DTOs.Store.Releasing
{
    public class UpdateSameDaySalesDTO
    {

        public int? StoreId { get; set; }

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? Id { get; set; }

        public string ORNumber { get; set; }

        public string DRNumber { get; set; }

        public string Remarks { get; set; }

        public string ClientName { get; set; }

        public string ContactNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string SalesAgent { get; set; }

        public string TransactionNo { get; set; }

    }
}
