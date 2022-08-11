using FC.Core.Domain.Common;

namespace FC.Api.DTOs.Store.Releasing
{
    public class BaseSearch : BaseGetAll
    {

        /// <summary>
        /// Holds current user storeid (User.StoreId)
        /// </summary>
        public int? StoreId { get; internal set; }

        /// <summary>
        /// Searched by STOrder.TransactionNo
        /// </summary>
        public string TransactionNo { get; set; }

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
        /// Searched by STSales.ClientName
        /// </summary>
        public string ClientName { get; set; }


        public ReleaseStatusEnum?[] ReleaseStatus { get; set; }

    }
}
