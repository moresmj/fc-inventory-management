using FC.Batch.DTOs.Item;

namespace FC.Batch.DTOs.Store
{
    public class STSalesDetailDTO
    {

        public int Id { get; set; }

        public int? STSalesId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public virtual ItemDTO Item { get; set; }

    }
}
