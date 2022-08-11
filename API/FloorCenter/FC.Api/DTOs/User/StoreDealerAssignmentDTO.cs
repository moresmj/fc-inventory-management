using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.User
{
    public class StoreDealerAssignmentDTO
    {

        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? StoreId { get; set; }
    }
}
