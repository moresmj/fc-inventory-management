using FC.Core.Domain.Items;
using System.Collections.Generic;

namespace FC.Api.Services.Items
{
    public interface ICategoryParentService
    {

        /// <summary>
        /// Get all parents, children and granchildren categories
        /// </summary>
        /// <returns>Parents, Children and Granchildren Category List</returns>
        IEnumerable<CategoryParent> GetAllParentsChildrenGrandChildrenCategories();


        /// <summary>
        /// Get parent children and grandchildren categories
        /// </summary>
        /// <param name="id">CategoryParent.Id</param>
        /// <returns>Parent Children and Grandchildre Category List</returns>
        IEnumerable<CategoryParent> GetParentAllChildrenGrandChildrenCategories(int? id);


        /// <summary>
        /// Get all parents and children categories
        /// </summary>
        /// <returns>Parents and Children Category List</returns>
        IEnumerable<CategoryParent> GetAllParentsAndChildrenCategories();


        /// <summary>
        /// Get parent all children categories
        /// </summary>
        /// <param name="id">CategoryParent.Id</param>
        /// <returns>Parent Children Category List</returns>
        IEnumerable<CategoryParent> GetParentAllChildrenCategories(int? id);
    }
}
