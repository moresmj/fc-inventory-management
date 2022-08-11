using FC.Core.Domain.Sizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Sizes
{
    public interface ISizeService
    {

        /// <summary>
        /// Get all sizes
        /// </summary>
        /// <returns>Size List</returns>
        IEnumerable<Size> GetAllSizes();

        void InsertSize(Size size);

    }
}
