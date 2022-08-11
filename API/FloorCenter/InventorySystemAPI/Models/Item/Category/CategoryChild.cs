using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Item.Category
{
    public class CategoryChild : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? CategoryParentId { get; set; }

        [Column(Order = 2)]
        public string Name { get; set; }

        public virtual ICollection<CategoryGrandChild> GrandChildren { get; set; }

    }
}
