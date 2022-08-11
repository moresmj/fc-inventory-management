using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Users
{
    public class StoreDealerAssignment : BaseEntity
    {
        #region Foreign Keys

        public int? UserId { get; set; }

        public int? StoreId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Store Store { get; set; }

        public virtual User User { get; set; }

        #endregion
    }
}
