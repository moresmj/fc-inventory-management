using FC.Core.Domain.Sizes;
using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.Items
{
    public class Item : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// Siz.Id
        /// </summary>
        public int? SizeId { get; set; }

        /// <summary>
        /// ItemAttribute.Id
        /// </summary>
        public int? ItemAttributeId { get; set; }

        /// <summary>
        /// CategoryParent.Id
        /// </summary>
        public int? CategoryParentId { get; set; }

        /// <summary>
        /// CategoryChild.Id
        /// </summary>
        public int? CategoryChildId { get; set; }

        /// <summary>
        /// CategoryGrandChild.Id
        /// </summary>
        public int? CategoryGrandChildId { get; set; }

        #endregion

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(30)]
        public string SerialNumber { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Tonality { get; set; }

        public decimal? SRP { get; set; }
        //Requested to be added by client
        public decimal? Cost { get; set; }

        public string Remarks { get; set; }

        [MaxLength(255)]
        public string ImageName { get; set; }

        public int? QtyPerBox { get; set; }

        [MaxLength(20)]
        public string BoxPerPallet { get; set; }

        #region Navigation Properties

        public Size Size { get; set; }

        public ItemAttribute ItemAttribute { get; set; }

        public CategoryParent Category { get; set; }

        public CategoryChild SubCategory { get; set; }

        public CategoryGrandChild SubSubCategory { get; set; }

        public ItemTypeEnum? ItemType { get; set; }

        public bool IsActive { get; set; }

        #endregion
    }
}
