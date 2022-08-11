using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Item.Category
{
    public class CategoryGrandChild : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? CategoryChildId { get; set; }

        [Column(Order = 2)]
        public string Name { get; set; }

    }
}
