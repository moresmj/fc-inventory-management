using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Warehouse.Inventory;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class WarehouseInventoriesController : BaseController
    {
        
        public WarehouseInventoriesController(FloorCenterContext context) 
            : base(context)
        { }

        [HttpGet]
        public IEnumerable<WHInventory> GetAll()
        {
            //TODO: Get records by logged in user warehouse id
            var warehouse_id = 1;

            var records = (from x in _context.WHInventories
                                .Include("DeliveredItems")
                                .AsNoTracking()
                           where x.WarehouseId == warehouse_id
                           select x);

            return records;
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //TODO: Get records by logged in user warehouse id
            var warehouse_id = 1;

            var obj = (from x in _context.WHInventories
               .Include("DeliveredItems")
                       where x.Id == id && x.WarehouseId == warehouse_id
                       select x).FirstOrDefault();


            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }


        [HttpGet]
        [Route("search")]
        public IEnumerable<WHInventory> Search(WHInventorySearch search)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(search);
            var records = (from x in _context.WHInventories.Where(x => _common.filterModelLike(x, criteria))
              .Include("DeliveredItems")
                           select x);

            return records;
        }


        [HttpPost]
        public IActionResult Add([FromBody] WHInventory inventory)
        {
            //TODO: Get logged in user warehouse id
            var warehouseId = 1;

            inventory.WarehouseId = warehouseId;

            var totalRecordCount = Convert.ToInt32(this._context.WHInventories.Where(x => x.WarehouseId == warehouseId).Count() + 1).ToString();

            inventory.TransactionNo = string.Format("P{0}", totalRecordCount.PadLeft(6, '0'));
            
            inventory.DateCreated = DateTime.Now;
            _context.WHInventories.Add(inventory);
            _context.SaveChanges();

            //  Update datecreated and Save stock records
            foreach(var item in inventory.DeliveredItems)
            {
                item.DateCreated = DateTime.Now;
                _context.WHInventoryDetails.Update(item);
                _context.SaveChanges();

                var stock = new WHStock
                {
                    WHInventoryDetailId = item.Id,
                    ItemId = item.ItemId,
                    OnHand = item.Quantity,
                    TransactionType = TransactionTypeEnum.PO,
                    DeliveryStatus = DeliveryStatusEnum.Delivered,
                    DateCreated = DateTime.Now
                };

                _context.WHStocks.Add(stock);
                _context.SaveChanges();
            }

            return new NoContentResult();
        }

    }
}