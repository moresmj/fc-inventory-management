using FC.Api.Helpers;
using FC.Core.Domain.Sizes;
using System;
using System.Collections.Generic;

namespace FC.Api.Services.Sizes
{
    public class SizeService : ISizeService
    {

        private DataContext _context;

        public SizeService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all sizes
        /// </summary>
        /// <returns>Size List</returns>
        public IEnumerable<Size> GetAllSizes()
        {
            return _context.Sizes;
        }


        /// <summary>
        /// Insert a size
        /// </summary>
        /// <param name="size">size</param>
        public void InsertSize(Size size)
        {
            if(String.IsNullOrEmpty(size.Name))
            {
                size.DateCreated = DateTime.Now;
            }


            _context.Sizes.Add(size);
            _context.SaveChanges();
        }
    }
}
