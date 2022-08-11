namespace FC.Core.Domain.Items
{
    public class CategoryGrandChild : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// CategoryChild.Id
        /// </summary>
        public int? CategoryChildId { get; set; }

        #endregion

        public string Name { get; set; }

    }
}
