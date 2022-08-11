using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.Users
{
    public class User : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Warehouse.Id
        /// </summary>
        public int? WarehouseId { get; set; }

        /// <summary>
        /// Store.Id
        /// </summary>
        public int? StoreId { get; set; }

        #endregion

        [MaxLength(255)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        [MaxLength(255)]
        public string EmailAddress { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(255)]
        public string ContactNumber { get; set; }

        public UserTypeEnum? UserType { get; set; }

        public AssignmentEnum? Assignment { get; set; }

        public DateTime? LastLogin { get; set; }

        public Boolean? IsActive { get; set; }

        #region Navigation Properties

        public virtual Store Store { get; set; }

        public virtual Warehouse Warehouse { get; set; }

        public ICollection<StoreDealerAssignment> StoreDealerAssignment { get; set; }

        #endregion


    }
}
