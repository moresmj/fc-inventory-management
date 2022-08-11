using System.Collections.Generic;
using System.Linq;
using FC.Api.Helpers;
using FC.Core.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Items
{
    public class CategoryParentService : ICategoryParentService
    {

        private DataContext _context;

        public CategoryParentService(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Get all parents, children and granchildren categories
        /// </summary>
        /// <returns>Parents, Children and Granchildren Category List</returns>
        public IEnumerable<CategoryParent> GetAllParentsChildrenGrandChildrenCategories()
        {
            return _context.CategoryParents.Include("Children").Include("Children.GrandChildren");
        }



        /// <summary>
        /// Get parent children and grandchildren categories
        /// </summary>
        /// <param name="id">CategoryParent.Id</param>
        /// <returns>Parent Children and Grandchildre Category List</returns>
        public IEnumerable<CategoryParent> GetParentAllChildrenGrandChildrenCategories(int? id)
        {
            return _context.CategoryParents.Where(x => x.Id == id).Include("Children").Include("Children.GrandChildren");
        }


        
        /// <summary>
        /// Get all parents and children categories
        /// </summary>
        /// <returns>Parents and Children Category List</returns>
        public IEnumerable<CategoryParent> GetAllParentsAndChildrenCategories()
        {
            return _context.CategoryParents.Include("Children");
        }


        /// <summary>
        /// Get parent all children categories
        /// </summary>
        /// <param name="id">CategoryParent.Id</param>
        /// <returns>Parent Children Category List</returns>
        public IEnumerable<CategoryParent> GetParentAllChildrenCategories(int? id)
        {
            return _context.CategoryParents.Where(x => x.Id == id).Include("Children");
        }


    }
}
