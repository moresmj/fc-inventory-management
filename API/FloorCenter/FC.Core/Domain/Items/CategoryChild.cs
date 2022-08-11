using System.Collections.Generic;

namespace FC.Core.Domain.Items
{
    public class CategoryChild : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// CategoryParent.Id
        /// </summary>
        public int? CategoryParentId { get; set; }

        #endregion

        public string Name { get; set; }

        public virtual ICollection<CategoryGrandChild> GrandChildren { get; set; }

    }
}
