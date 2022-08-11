using FC.Core.Domain.Items;
using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.Sizes
{
    public class Size : BaseEntity
    {
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
