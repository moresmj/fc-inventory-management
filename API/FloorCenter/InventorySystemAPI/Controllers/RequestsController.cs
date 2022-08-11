using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Request;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Store.Stock;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAPI.Controllers
{

    [Route("api/[controller]")]
    public class RequestsController : BaseController
    {

        public RequestsController(FloorCenterContext context) : base(context)
        {

        }

        [HttpGet]
        [Route("orders/pending")]
        public IEnumerable<STInventory> OrdersPending(STInventorySearch search)
        {

            search.RequestStatus = RequestStatusEnum.Pending;

            var criteria = _common.getModelPropertyWithValueAsDictionary(search);
            var records = (from x in _context.STInventories.Where(x => _common.filterModelLike(x, criteria))
                           .Include("RequestedItems")
                           .Include("RequestedItems.Item")
                           select x);

            return records;
        }


        [HttpGet("{id}")]
        [Route("orders/pending/{id}")]
        public IActionResult OrdersPending_GetById(int id)
        {
            var obj = _context.STInventories.Include("RequestedItems").Include("RequestedItems.Item").FirstOrDefault(x => x.Id == id && x.RequestStatus == RequestStatusEnum.Pending);
            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }


        [HttpPut("{id}")]
        [Route("orders/pending/update/{id}")]
        public IActionResult OrdersPending_Update(int id, [FromBody] PendingOrder order)
        {
            if (order.Id != id)
            {
                return BadRequest();
            }

            var obj = (from x in _context.STInventories
                           .Include("RequestedItems")
                       where x.Id == id && x.RequestStatus == RequestStatusEnum.Pending
                       select x).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            obj.DateUpdated = DateTime.Now;
            obj.RequestStatus = RequestStatusEnum.Approved;
            _context.STInventories.Update(obj);
            _context.SaveChanges();


            foreach (var item in order.RequestedItems)
            {

                var reqItem = obj.RequestedItems.FirstOrDefault(x => x.Id == item.Id && x.STInventoryId == item.STInventoryId && x.ItemId == item.ItemId);
                if (reqItem == null)
                {
                    continue;
                }

                reqItem.ApprovedQuantity = item.ApprovedQuantity;
                reqItem.ApprovedRemarks = item.ApprovedRemarks;
                reqItem.DeliveryStatus = DeliveryStatusEnum.Waiting;
                reqItem.DateUpdated = DateTime.Now;
                _context.STInventoryDetails.Update(reqItem);
                _context.SaveChanges();
                
            }


            return new NoContentResult();
        }

    }
}