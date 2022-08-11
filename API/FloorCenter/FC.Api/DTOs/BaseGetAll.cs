using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs
{
    public abstract class BaseGetAll
    {

        public BaseGetAll()
        {
            CurrentPage = 1;
        }

        public int CurrentPage { get; set; }

        public bool ShowAll { get; set; }
    }
}
