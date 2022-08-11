using System.Collections.Generic;
using System.Linq;
using FC.Api.Helpers;
using FC.Core.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Items
{
    public class CategoryChildService : ICategoryChildService
    {

        private DataContext _context;

        public CategoryChildService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all child and grandchild categories
        /// </summary>
        /// <returns>Child and Grandchild Category List</returns>
        public IEnumerable<CategoryChild> GetAllChildGrandChildCategories()
        {
            return _context.CategoryChildren.Include("GrandChildren");
        }



        public IEnumerable<CategoryChild> GetChildAllGrandChildCategories(int? id)
        {
            return _context.CategoryChildren.Where(x => x.Id == id).Include("GrandChildren");
        }
    }
}
