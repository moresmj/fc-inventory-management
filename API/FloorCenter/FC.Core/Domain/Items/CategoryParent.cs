using System.Collections.Generic;

namespace FC.Core.Domain.Items
{
    public class CategoryParent : BaseEntity
    {

        public string Name { get; set; }

        public virtual ICollection<CategoryChild> Children { get; set; }

    }
}
