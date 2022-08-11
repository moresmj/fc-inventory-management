namespace FC.Api.DTOs.Store
{
    public class AddSalesOrderDeliveryItems
    {

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? STSalesId { get; set; }

        /// <summary>
        /// STSalesDetail.Id
        /// </summary>
        public int? STSalesDetailId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

    }
}
