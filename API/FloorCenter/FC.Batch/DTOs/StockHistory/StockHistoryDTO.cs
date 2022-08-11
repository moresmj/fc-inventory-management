using System;

namespace FC.Batch.DTOs.StockHistory
{
    public class StockHistoryDTO
    {
        public string TransactionNo { get; set; }

        public string Transaction { get; set; }

        public string PONumber { get; set; }

        public string DRNumber { get; set; }

        public string ORNumber { get; set; }

        public string SINumber { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public DateTime? PODate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public DateTime? SalesDate { get; set; }

        public DateTime? TransactionDate { get; set; }

        public int? Stock { get; set; }


        public string Code { get; set; }

        public string Description { get; set; }

        public string SizeName { get; set; }

        public int? Adjustment { get; set; }

        public int? FromSupplier { get; set; }

        public int? FromInterBranch { get; set; }

        public int? FromSalesReturns { get; set; }

        public int? RTV { get; set; }

        public bool? Broken { get; set; }

    }
}
