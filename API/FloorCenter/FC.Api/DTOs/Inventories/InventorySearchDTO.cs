using FC.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Inventories
{
    public class InventorySearchDTO : BaseGetAll
    {
        public int? Id { get; set; }

        public string SerialNumber { get; set; }

        public string Code { get; set; }

        public string ItemName { get; set; }

        public string Description { get; set; }

        public int? SizeId { get; set; }

        public string Tonality { get; set; }


        // Commonly used on Main Site search
        public int? WarehouseId { get; set; }

        public int? StoreId { get; set; }

        public string Keyword { get; set; }

        public int? Keyword2 { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }


        // Used on Store and Warehouse Site
        public bool OnlyAvailableStocks { get; set; }

        public bool IsOutOfStocks { get; set; }

        public bool HasBroken { get; set; }

        public UserTypeEnum? UserType { get; set; }

        public int? UserId { get; set; }

        

    }
}
