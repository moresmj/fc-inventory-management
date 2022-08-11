using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store;
using InventorySystemAPI.Models.Store.Released;
using InventorySystemAPI.Models.Store.Releasing;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.Store.Stock;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class StoreController : BaseController
    {

        private readonly Helpers.Store.DataHelper _dataHelper;

        public StoreController(FloorCenterContext context)
        : base(context)
        {
            this._dataHelper = new Helpers.Store.DataHelper(context);
        }


        [HttpPost]
        [Route("sales/add")]
        public IActionResult Add([FromBody] STSales model)
        {
            //TODO: Get logged in user store id
            var store_id = 1;

            model.StoreId = store_id;
            model.DateCreated = DateTime.Now;

            foreach (var item in model.OrderedItems)
            {
                item.DeliveryStatus = DeliveryStatusEnum.Waiting;
                item.DateCreated = DateTime.Now;
            }

            _context.STSales.Add(model);
            _context.SaveChanges();

            foreach (var item in model.OrderedItems)
            {
                var stock = new STStock
                {
                    STSalesDetailId = item.Id,
                    ItemId = item.ItemId,
                    OnHand = -item.Quantity,
                    DateCreated = DateTime.Now
                };

                this._context.STStocks.Add(stock);
            }

            _context.SaveChanges();

            return new NoContentResult();
        }


        /// <summary>
        /// Get all sales which are not yet released
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sales")]
        public IEnumerable<STSales> GetAllSales()
        {

            return this._dataHelper.GetAll(DeliveryStatusEnum.Waiting);

        }

        [HttpGet("{id}")]
        [Route("releasing/for_pickup/{id}")]
        public IActionResult ReleasingForPickup(int? id)
        {

            var search = new ReleasingSearch();

            search.Id = id;

            //TODO: Get logged in user store id
            var store_id = 1;

            search.StoreId = store_id;

            search.DeliveryType = DeliveryTypeEnum.Pickup;

            var obj = this._dataHelper.GetSingle(search, DeliveryStatusEnum.Waiting);
            
            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }


        [HttpPut("{id}")]
        [Route("releasing/for_pickup/add/{id}")]
        public IActionResult ReleasingForPickup(int? id, [FromBody] STRelease model)
        {
            var obj = this.SearchRecordForReleasing(id, DeliveryTypeEnum.Pickup);
            if (obj == null)
            {
                return NotFound();
            }

            this.SaveReleasingRecord(model, DeliveryTypeEnum.Pickup);

            return new NoContentResult();

        }
        

        private STSales SearchRecordForReleasing(int? id, DeliveryTypeEnum deliveryType)
        {
            var search = new ReleasingSearch();

            search.Id = id;

            //TODO: Get logged in user store id
            var store_id = 1;

            search.StoreId = store_id;

            search.DeliveryType = deliveryType;

            var obj = this._dataHelper.GetSingle(search, DeliveryStatusEnum.Waiting);
            return obj;
        }


        private void SaveReleasingRecord(STRelease model, DeliveryTypeEnum deliveryType)
        {
            model.DateCreated = DateTime.Now;
            model.DeliveryType = deliveryType;
            if(deliveryType != DeliveryTypeEnum.Pickup)
            {
                model.DateReleased = null;
            }

            foreach (var item in model.ReleasedItems)
            {
                //  Check if release quantity is zero
                if (item.Quantity == 0)
                {
                    //Skip record
                    continue;
                }


                item.DateCreated = DateTime.Now;

                var totalRelease = Convert.ToInt32(
                                                        this._context.STReleaseDetails
                                                        .Where(x => x.STSalesDetailId == item.STSalesDetailId)
                                                        .Sum(x => x.Quantity)
                                                    ) + item.Quantity;

                var soldItem = this._context.STSalesDetails
                               .Where(x => x.Id == item.STSalesDetailId)
                               .FirstOrDefault();

                if(soldItem == null)
                {
                    continue;
                }

                if (soldItem.Quantity == totalRelease)
                {
                    soldItem.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    soldItem.DateUpdated = DateTime.Now;

                    _context.STSalesDetails.Update(soldItem);
                }
            }

            //  Filter only record with release quantity more than zero (0)
            model.ReleasedItems = model.ReleasedItems.Where(x => x.Quantity > 0).ToList();

            _context.STReleases.Add(model);
            _context.SaveChanges();
        }


        [HttpGet]
        [Route("released")]
        public IEnumerable<STSales> GetAllReleased(ReleasedSearch model)
        {

            //TODO: Get logged in user store id
            var store_id = 1;

            model.StoreId = store_id;

            return this._dataHelper.GetAll(DeliveryStatusEnum.Delivered);
            
        }


        [HttpGet("{id}")]
        [Route("releasing/store_delivery/{id}")]
        public IActionResult ReleasingStoreDelivery(int? id)
        {
            var search = new ReleasingSearch();

            search.Id = id;

            //TODO: Get logged in user store id
            var store_id = 1;

            search.StoreId = store_id;

            search.DeliveryType = DeliveryTypeEnum.Store;

            var obj = this._dataHelper.GetSingle(search, DeliveryStatusEnum.Waiting);

            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }

        [HttpPut("{id}")]
        [Route("releasing/store_delivery/add/{id}")]
        public IActionResult ReleasingStoreDelivery(int? id, [FromBody] STRelease model)
        {
            var obj = this.SearchRecordForReleasing(id, DeliveryTypeEnum.Store);
            if (obj == null)
            {
                return NotFound();
            }
            
            this.SaveReleasingRecord(model, DeliveryTypeEnum.Store);

            return new NoContentResult();
        }


        [HttpGet("{id}")]
        [Route("releasing/warehouse_delivery/{id}")]
        public IActionResult ReleasingWarehouseDelivery(int? id)
        {
            var search = new ReleasingSearch();

            search.Id = id;

            //TODO: Get logged in user store id
            var store_id = 1;

            search.StoreId = store_id;

            search.DeliveryType = DeliveryTypeEnum.Warehouse;

            var obj = this._dataHelper.GetSingle(search, DeliveryStatusEnum.Waiting);

            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }

        [HttpPut("{id}")]
        [Route("releasing/warehouse_delivery/add/{id}")]
        public IActionResult ReleasingWarehouseDelivery(int? id, [FromBody] STRelease model)
        {
            var obj = this.SearchRecordForReleasing(id, DeliveryTypeEnum.Warehouse);
            if (obj == null)
            {
                return NotFound();
            }

            this.SaveReleasingRecord(model, DeliveryTypeEnum.Warehouse);

            return new NoContentResult();
        }


        [HttpGet("{id}")]
        [Route("inventory")]
        public IEnumerable<STStock> Inventory()
        {
            var search = new InventorySearch();

            //TODO: Get logged in user store id
            var store_id = 1;

            search.StoreId = store_id;

            var criteria = _common.getModelPropertyWithValueAsDictionary(search);

            var records = (from x in _context.STStocks.Where(x => _common.filterModelEqual(x, criteria))
                           .Include(y => y.Item)
                           select x);

            return records;

        }

    }
}