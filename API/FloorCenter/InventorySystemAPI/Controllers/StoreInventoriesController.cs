using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAPI.Controllers
{

    [Route("api/[controller]")]
    public class StoreInventoriesController : BaseController
    {

        public StoreInventoriesController(FloorCenterContext context) : base(context)
        {

        }

        [HttpPost]
        [Route("request/add")]
        public IActionResult Add([FromBody] STInventory inventory)
        {
            //TODO: Get logged in user store id
            var store_id = 1;

            var totalRecordCount = Convert.ToInt32(this._context.STInventories.Where(x => x.StoreId == store_id && x.TransactionType == inventory.TransactionType).Count() + 1).ToString();

            var trans = "PO";
            if(inventory.TransactionType == TransactionTypeEnum.PhysicalCount)
            {
                trans = "PC";
            }
            else if(inventory.TransactionType == TransactionTypeEnum.Request)
            {
                trans = "RE";
            }
            else if (inventory.TransactionType == TransactionTypeEnum.Return)
            {
                trans = "RT";
            }
            else if (inventory.TransactionType == TransactionTypeEnum.Sales)
            {
                trans = "SA";
            }
            else if (inventory.TransactionType == TransactionTypeEnum.Transfer)
            {
                trans = "TR";
            }

            inventory.TransactionNo = string.Format("{0}{1}", trans, totalRecordCount.PadLeft(6, '0'));
            
            inventory.StoreId = store_id;
            inventory.RequestStatus = RequestStatusEnum.Pending;
            inventory.DateCreated = DateTime.Now;

            foreach(var detail in inventory.RequestedItems)
            {
                detail.DeliveryStatus = DeliveryStatusEnum.Pending;
                detail.DateCreated = DateTime.Now;
            }

            _context.STInventories.Add(inventory);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<STInventory> Search(STInventorySearch user)
        {
            _common.dateToBeFiltered = new string[] { "PODate" }; 
            var criteria = _common.getModelPropertyWithValueAsDictionary(user);
            var records = (from x in _context.STInventories.Where(x => _common.filterModelLike(x, criteria))
                           .Include("Warehouse")
                           .Include("RequestedItems")
                           .Include("RequestedItems.Item")
                           .Include("RequestedItems.Item.Size")
                           select x);

            return records;
        }

    }
}