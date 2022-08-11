using FC.Core.Domain.Items;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;

namespace FC.Batch.DTOs.Item
{
    public class ItemDTO
    {

        public int Id { get; set; }

        public int? SizeId { get; set; }

        public int? ItemAttributeId { get; set; }

        public int? CategoryParentId { get; set; }

        public int? CategoryChildId { get; set; }

        public int? CategoryGrandChildId { get; set; }

        [DisplayName("Item Code")]
        public string Code { get; set; }

        public string SerialNumber { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Tonality { get; set; }

        public decimal? SRP { get; set; }

        public string Remarks { get; set; }

        public string ImageName { get; set; }


        public FC.Core.Domain.Sizes.Size Size { get; set; }

        public CategoryParent Category { get; set; }

        public CategoryChild SubCategory { get; set; }

        public CategoryGrandChild SubSubCategory { get; set; }

        public int QtyPerBox { get; set; }

        public string BoxPerPallet { get; set; }

        public ItemTypeEnum ItemType { get; set; }
        
        public DateTime? DateCreated { get; set; }

    }
}
