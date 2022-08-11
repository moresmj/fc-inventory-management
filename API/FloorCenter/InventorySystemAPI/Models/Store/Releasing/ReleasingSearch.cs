using InventorySystemAPI.Classes;
using InventorySystemAPI.Models.Store.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystemAPI.Models.Store.Releasing
{
    public class ReleasingSearch : BaseSearch
    {

        public int? Id { get; set; }

        public int? StoreId { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

    }
}
