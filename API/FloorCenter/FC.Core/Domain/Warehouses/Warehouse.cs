using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.Warehouses
{
    public class Warehouse : BaseEntity
    {
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(255)]
        public string ContactNumber { get; set; }

        public bool Vendor { get; set; }

    }
}
