using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Item;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Item
{

    [Validator(typeof(ItemValidator))]
    public class Item : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        #region Foreign Keys

        [Column(Order = 1)]
        public int? SizeId { get; set; }

        [Column(Order = 2)]
        public int? ItemAttributeId { get; set; }

        [Column(Order = 3)]
        public int? CategoryParentId { get; set; }

        [Column(Order = 4)]
        public int? CategoryChildId { get; set; }

        [Column(Order = 5)]
        public int? CategoryGrandChildId { get; set; }

        #endregion

        [Column(Order = 6)]
        public string Code { get; set; }

        [Column(Order = 7)]
        public int? SerialNumber { get; set; }

        [Column(Order = 8)]
        public string Name { get; set; }

        [Column(Order = 9)]
        public string Description { get; set; }

        [Column(Order = 10)]
        public string Tonality { get; set; }

        [Column(Order = 11)]
        public string Remarks { get; set; }

        //public byte[] Image { get; set; }

        #region Navigation Properties

        public virtual Models.Size.Size Size { get; set; }

        public virtual Models.Item.Attribute.ItemAttribute ItemAttribute { get; set; }

        public virtual Models.Item.Category.CategoryParent CategoryParent { get; set; }

        public virtual Models.Item.Category.CategoryChild CategoryChild { get; set; }

        public virtual Models.Item.Category.CategoryGrandChild CategoryGrandChild { get; set; }

        #endregion

    }
}
