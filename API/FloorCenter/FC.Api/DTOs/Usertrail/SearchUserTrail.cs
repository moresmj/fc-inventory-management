using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Usertrail
{
    public class SearchUserTrail : BaseGetAll
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? UserID { get; set; }

    }
}
