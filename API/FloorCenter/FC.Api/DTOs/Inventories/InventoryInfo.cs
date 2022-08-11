using FC.Api.DTOs.Store;
using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Inventories
{
    public class InventoryInfo
    {
        public int? ItemId { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }
        public string ItemName { get; set; }
        public string SizeName { get; set; }
        public string Tonality { get; set; }
        public decimal? SRP { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseAddress { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string ImageName { get; set; }
        public int OnHand { get; set; }
        public int ForRelease { get; set; }
        public int Available { get; set; }
        public int Broken { get; set; }
        public int ForRTV { get; set; }
        public STImport ImportDetails { get; set; }
        public int? adjustment { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? physicalCount { get; set; }
    }
}
