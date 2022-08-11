using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs
{
    public class ReturnErrorModel
    {
        public string Status { get; set; }

        public string Errors { get; set; }
    }
}
