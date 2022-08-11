using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystemAPI.Models.Item
{
    public class ItemSearch
    {
        public string Code { get; set; }

        public int? SerialNumber { get; set; }

        public string Tonality { get; set; }
    }
}
