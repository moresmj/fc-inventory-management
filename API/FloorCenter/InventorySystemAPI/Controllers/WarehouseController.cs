using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Warehouse.DeliveryRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class WarehouseController : BaseController
    {

        public WarehouseController(FloorCenterContext context)
        : base(context)
        { }

        [HttpGet]
        [Route("delivery_requests/list")]
        public IEnumerable<STInventory> ForDelivery()
        {
            var search = new DeliveryListSearch();
            search.RequestStatus = RequestStatusEnum.Approved;

            //TODO: Get logged in user warehouse id
            var warehouse_id = 1;

            search.WarehouseId = warehouse_id;

            var criteria = _common.getModelPropertyWithValueAsDictionary(search);

            var records = (from x in _context.STInventories.Where(x => _common.filterModelEqual(x, criteria))
                           .Include(y => y.RequestedItems)
                           select new STInventory
                           {
                               Id = x.Id,
                               StoreId = x.StoreId,
                               WarehouseId = x.WarehouseId,
                               PONumber = x.PONumber,
                               TransactionType = x.TransactionType,
                               Remarks = x.Remarks,
                               RequestStatus = x.RequestStatus,
                               PODate = x.PODate,
                               RequestedItems = x.RequestedItems//.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                           })/*.Where(x => x.RequestedItems.Count > 0)*/;

            return records;
            
        }

    }
}
