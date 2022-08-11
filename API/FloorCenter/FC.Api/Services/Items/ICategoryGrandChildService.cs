using FC.Core.Domain.Items;
using System.Collections.Generic;

namespace FC.Api.Services.Items
{
    public interface ICategoryGrandChildService
    {

        /// <summary>
        /// Get all granchild categories
        /// </summary>
        /// <returns>Grandchild Category List</returns>
        IEnumerable<CategoryGrandChild> GetAllGrandChildCategories();

    }
}
