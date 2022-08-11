using FC.Core.Domain.Items;
using System.Collections.Generic;

namespace FC.Api.Services.Items
{
    public interface ICategoryChildService
    {

        /// <summary>
        /// Get all child and grandchildren categories
        /// </summary>
        /// <returns>Child and Grandchild Category List</returns>
        IEnumerable<CategoryChild> GetAllChildGrandChildCategories();


        /// <summary>
        /// Get child all grandchildren categories
        /// </summary>
        /// <param name="id">CategoryChild.Id</param>
        /// <returns>Child and Grandchild Category List</returns>
        IEnumerable<CategoryChild> GetChildAllGrandChildCategories(int? id);

    }
}
