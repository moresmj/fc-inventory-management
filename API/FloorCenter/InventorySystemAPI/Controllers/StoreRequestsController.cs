using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Store.Request;
using InventorySystemAPI.Models.Store.Stock;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class StoreRequestsController : BaseController
    {

        public StoreRequestsController(FloorCenterContext context)
            : base(context)
        {

        }

        [HttpGet]
        [Route("for_delivery")]
        public IEnumerable<STInventory> ForDelivery()
        {
            var search = new ForDeliverySearch();
            search.RequestStatus = RequestStatusEnum.Approved;

            //TODO: Get logged in user store id
            var store_id = 1;

            search.StoreId = store_id;

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
                               RequestedItems = x.RequestedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                           }).Where(x => x.RequestedItems.Count > 0);

            return records;


        }

        [HttpGet("{id}")]
        [Route("for_delivery/{id}")]
        public IActionResult ForDelivey_Add(int? id)
        {
            //TODO: Get logged in user store id
            var store_id = 1;
            
            var obj = (from x in _context.STInventories.Where(x => x.Id == id && x.StoreId == store_id && x.RequestStatus == RequestStatusEnum.Approved)
                      .Include("RequestedItems")
                      .Include("RequestedItems.Item")
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
                          RequestedItems = x.RequestedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                      }).Where(y => y.RequestedItems.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            foreach (var item in obj.RequestedItems)
            {
                item.Item = this._context.Items.Where(x => x.Id == item.ItemId).FirstOrDefault();
                if(item.Item != null)
                {
                    if(item.Item.SizeId.HasValue)
                    {
                        item.Item.Size = this._context.Sizes.Where(x => x.Id == item.Item.SizeId).FirstOrDefault();
                    }
                }
            }

            return new ObjectResult(obj);
        }


        [HttpPut("{id}")]
        [Route("for_delivery/add/{id}")]
        public IActionResult ForDelivey_Add(int id, [FromBody] STDelivery model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }
            
            model.Id = 0;
            model.DateCreated = DateTime.Now;
            model.STInventoryId = id;

            //TODO: Get logged in user store id
            var store_id = 1;

            model.StoreId = store_id;

            foreach (var item in model.ItemsToBeDelivered)
            {

                item.Id = 0;
                item.DateCreated = DateTime.Now;
                item.DeliveryStatus = DeliveryStatusEnum.Waiting;
                _context.STDeliveryDetails.Add(item);
                _context.SaveChanges();

                var wareStock = new WHStock
                {
                    STDeliveryDetailId = item.Id,
                    ItemId = item.ItemId,
                    OnHand = -item.Quantity,
                    TransactionType = TransactionTypeEnum.PO,
                    DeliveryStatus = DeliveryStatusEnum.Waiting,
                    DateCreated = DateTime.Now
                };

                _context.WHStocks.Add(wareStock);

            }

            _context.STDeliveries.Add(model);
            _context.SaveChanges();


            return new NoContentResult();
        }



        [HttpGet]
        [Route("incoming_delivery")]
        public IEnumerable<STDelivery> IncomingDelivery()
        {
            //TODO: Get logged in user store id
            var store_id = 1;

            var records = (from x in _context.STDeliveries.Where(x => x.StoreId == store_id)
                      .Include(y => y.ItemsToBeDelivered)
                       select new ForDeliveryUpdate
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           STInventoryId = x.STInventoryId,
                           DeliveryDate = x.DeliveryDate,
                           ReceivedBy = x.ReceivedBy,
                           STInventory = x.STInventory,
                           ItemsToBeDelivered = x.ItemsToBeDelivered.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.ItemsToBeDelivered.Count > 0);


            foreach (var dtl in records)
            {
                dtl.STInventory = this._context.STInventories.Where(x => x.Id == dtl.STInventoryId).FirstOrDefault();
            }

            return records;
        }


        [HttpGet("{id}")]
        [Route("incoming_delivery/{id}")]
        public IActionResult IncomingDelivery_Update(int id)
        {
            //TODO: Get logged in user store id
            var store_id = 1;

            var obj = (from x in _context.STDeliveries.Where(x => x.Id == id && x.StoreId == store_id)
                      .Include("ItemsToBeDelivered")
                      .Include("ItemsToBeDelivered.Item")
                      .AsNoTracking()
                       select new ForDeliveryUpdate
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           STInventoryId = x.STInventoryId,
                           DeliveryDate = x.DeliveryDate,
                           ReceivedBy = x.ReceivedBy,
                           ItemsToBeDelivered = x.ItemsToBeDelivered.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.ItemsToBeDelivered.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            foreach(var dtl in obj.ItemsToBeDelivered)
            {
                obj.STInventory = this._context.STInventories.Where(x => x.Id == obj.STInventoryId).FirstOrDefault();

                dtl.Item = this._context.Items.Where(x => x.Id == dtl.ItemId).FirstOrDefault();
                if(dtl.Item != null)
                {
                    if(dtl.Item.SizeId.HasValue)
                    {
                        dtl.Item.Size = this._context.Sizes.Where(x => x.Id == dtl.Item.SizeId).FirstOrDefault();
                    }
                }
            }
            
            return new ObjectResult(obj);
        }


        [HttpPut("{id}")]
        [Route("incoming_delivery/update/{id}")]
        public IActionResult IncomingDelivery_Update(int id, [FromBody] ForDeliveryUpdate model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }

            //TODO: Get logged in user store id
            var store_id = 1;

            var obj = (from x in _context.STDeliveries.Where(x => x.Id == id && x.StoreId == store_id)
                      .Include(y => y.STInventory)
                      .Include(y => y.ItemsToBeDelivered)
                      .AsNoTracking()
                       select new ForDeliveryUpdate
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           DeliveryDate = x.DeliveryDate,
                           ItemsToBeDelivered = x.ItemsToBeDelivered.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.ItemsToBeDelivered.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }


            foreach(var item in model.ItemsToBeDelivered)
            {
                //  Check if delivered quantity is zero
                if(item.DeliveredQuantity == 0)
                {
                    item.DeliveryStatus = DeliveryStatusEnum.NotDelivered;
                }
                else
                {
                    item.DeliveryStatus = DeliveryStatusEnum.Delivered;

                    //  Get total delivered quantity and current quantity delivered
                    var totalDeliveredItems = Convert.ToInt32(this._context.STDeliveryDetails
                                                .Where(x => x.STInventoryDetailId == item.STInventoryDetailId)
                                                .Sum(x => x.DeliveredQuantity)) + item.DeliveredQuantity;


                    //  Get requested item to warehouse record
                    var reqItemToWarehouse = this._context.WHStocks.Where(x => x.STDeliveryDetailId == item.Id).FirstOrDefault();

                    //  Check if totalDeliveredItems and requested item to warehouse
                    //  are equal
                    if(totalDeliveredItems == Math.Abs(reqItemToWarehouse.OnHand.Value))
                    {
                        //  Mark warehouse request as delivered
                        reqItemToWarehouse.DeliveryStatus = DeliveryStatusEnum.Delivered;

                        reqItemToWarehouse.DateUpdated = DateTime.Now;

                        this._context.WHStocks.Update(reqItemToWarehouse);
                    }

                    var stStock = new STStock
                    {
                        STDeliveryDetailId = item.Id,
                        ItemId = item.ItemId,
                        OnHand = item.DeliveredQuantity,
                        DateCreated = DateTime.Now
                    };

                    this._context.STStocks.Add(stStock);

                }


                item.DateUpdated = DateTime.Now;
                this._context.STDeliveryDetails.Update(item);

                this._context.SaveChanges();
            }


            return new NoContentResult();
        }
    }
}