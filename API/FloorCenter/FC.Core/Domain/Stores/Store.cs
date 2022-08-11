using FC.Core.Domain.Companies;
using FC.Core.Domain.Warehouses;
using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.Stores
{
    public class Store : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Company.Id
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        #endregion

        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(255)]
        public string ContactNumber { get; set; }

        #region Navigation Properties

        public virtual Company Company { get; set; }

        public virtual Warehouse Warehouse { get; set; }

        #endregion

    }
}
