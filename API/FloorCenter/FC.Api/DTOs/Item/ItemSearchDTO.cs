namespace FC.Api.DTOs.Item
{
    public class ItemSearchDTO : BaseGetAll
    {

        public int Id { get; set; }

        public string Code { get; set; }

        public string SerialNumber { get; set; }

        public string Tonality { get; set; }

        public int? SizeId { get; set; }

        public int? CategoryParentId { get; set; }

        public int? CategoryChildId { get; set; }

        public int? CategoryGrandChildId { get; set; }


        /// <summary>
        /// Search records by keyword. 
        /// Keyword should be at least 3 characters
        /// </summary>
        public string Keyword { get; set; }




    }
}
