using System.Collections.Generic;
using FC.Api.Helpers;
using FC.Core.Domain.Items;

namespace FC.Api.Services.Items
{
    public class CategoryGrandChildService : ICategoryGrandChildService
    {

        private DataContext _context;

        public CategoryGrandChildService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all granchild categories
        /// </summary>
        /// <returns>Grandchild Category List</returns>
        public IEnumerable<CategoryGrandChild> GetAllGrandChildCategories()
        {
            throw new System.NotImplementedException();
        }
    }
}
