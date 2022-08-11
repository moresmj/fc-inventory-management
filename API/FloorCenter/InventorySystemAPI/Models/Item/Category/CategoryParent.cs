using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Item.Category
{
    public class CategoryParent : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string Name { get; set; }

        public  virtual ICollection<CategoryChild> Children { get; set; }

    }
}
