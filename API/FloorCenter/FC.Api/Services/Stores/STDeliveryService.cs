using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Stores
{
    public class STDeliveryService : ISTDeliveryService
    {


        private DataContext _context;

        public STDeliveryService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Insert delivery
        /// </summary>
        /// <param name="stdelivery">STDelivery</param>
        public void InsertDeliveryToShowroom(STDelivery stdelivery)
        {
            var order = _context.STOrders.Where(p => p.Id == stdelivery.Id).FirstOrDefault();
            var warehouse = _context.Warehouses.Where(p => p.Id == order.WarehouseId).FirstOrDefault();

            if (order != null)
            {

                stdelivery.STOrderId = stdelivery.Id;
                stdelivery.Id = 0;
                stdelivery.DateCreated = DateTime.Now;

                if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {
                    stdelivery.StoreId = order.StoreId;
                    stdelivery.DeliveryFromStoreId = order.OrderToStoreId;

                    var sales = _context.STSales.Where(p => p.STOrderId == order.Id).FirstOrDefault();
                    if (sales != null)
                    {
                        stdelivery.STSalesId = sales.Id;
                    }
                }

                var isVendor = false;
                if (warehouse != null)
                {
                    if (warehouse.Vendor == true)
                    {
                        isVendor = true;
                    }

                }

                foreach (var delivery in stdelivery.ShowroomDeliveries)
                {

                    //  Check if request deliver quantity is 0
                    if (delivery.Quantity == 0)
                    {
                        //  Skip record
                        continue;
                    }

                    delivery.Id = 0;
                    delivery.DateCreated = DateTime.Now;
                    delivery.DeliveryStatus = DeliveryStatusEnum.Pending;
                    delivery.ReleaseStatus = ReleaseStatusEnum.Pending;

                    if (isVendor)
                    {
                        delivery.DeliveryStatus = DeliveryStatusEnum.Waiting;
                        delivery.ReleaseStatus = ReleaseStatusEnum.Released;
                    }
                    _context.STShowroomDeliveries.Add(delivery);
                    _context.SaveChanges();
                }

                //  Filter only record with request deliver quantity more than 0
                stdelivery.ShowroomDeliveries = stdelivery.ShowroomDeliveries.Where(p => p.Quantity > 0).ToList();

                _context.STDeliveries.Add(stdelivery);
                _context.SaveChanges();

               

                if (!isVendor)
                {
                    if (order.OrderType == OrderTypeEnum.ShowroomStockOrder ||
                        (order.OrderType == OrderTypeEnum.ClientOrder && order.DeliveryType == DeliveryTypeEnum.ShowroomPickup))
                    {
                        var whStockService = new WHStockService(_context);

                        foreach (var delivery in stdelivery.ShowroomDeliveries)
                        {
                            var whstock = new WHStock
                            {
                                WarehouseId = order.WarehouseId,
                                STShowroomDeliveryId = delivery.Id,
                                ItemId = delivery.ItemId,
                                OnHand = -delivery.Quantity,
                                TransactionType = TransactionTypeEnum.PO,
                                DeliveryStatus = DeliveryStatusEnum.Pending,
                                ReleaseStatus = ReleaseStatusEnum.Pending
                            };

                            whStockService.InsertStock(whstock);
                        }
                    }
                    else if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder && order.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                    {
                        var stStockService = new STStockService(_context);

                        foreach (var delivery in stdelivery.ShowroomDeliveries)
                        {
                            var ststock = new STStock
                            {
                                StoreId = order.OrderToStoreId,
                                STShowroomDeliveryId = delivery.Id,
                                ItemId = delivery.ItemId,
                                OnHand = -delivery.Quantity,
                                DeliveryStatus = DeliveryStatusEnum.Pending,
                                ReleaseStatus = ReleaseStatusEnum.Pending
                            };

                            stStockService.InsertSTStock(ststock);
                        }
                    }
                }
            }

        }



        /// <summary>
        /// Update delivery
        /// </summary>
        /// <param name="param">STDelivery</param>
        public void UpdateDelivery(STDelivery param)
        {
            var delivery = _context.STDeliveries
                                .Include(p => p.ShowroomDeliveries) 
                                .Include(p => p.ClientDeliveries)
                                .Where(x => x.Id == param.Id)
                            .SingleOrDefault();

            delivery.ApprovedDeliveryDate = param.ApprovedDeliveryDate;
            delivery.DriverName = param.DriverName;
            delivery.PlateNumber = param.PlateNumber;
            delivery.DateUpdated = DateTime.Now;
            //delivery.Delivered = DeliveryStatusEnum.Waiting;

            var order = _context.STOrders.Where(p => p.Id == delivery.STOrderId).FirstOrDefault();

            if (delivery.ShowroomDeliveries != null && delivery.ShowroomDeliveries.Count > 0)
            {
                foreach (var showroom in delivery.ShowroomDeliveries)
                {
                    showroom.DateUpdated = DateTime.Now;
                    //  Change from Pending to Waiting
                    showroom.DeliveryStatus = DeliveryStatusEnum.Waiting;
                    showroom.ReleaseStatus = ReleaseStatusEnum.Waiting;

                    _context.STShowroomDeliveries.Update(showroom);
                    _context.SaveChanges();

                    if (order != null)
                    {
                        if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                        {
                            this.UpdateSTStock(showroom.Id);
                        }
                        else
                        {
                            this.UpdateWHStock(showroom.Id);
                        }
                    }
                    else
                    {
                        this.UpdateWHStock(showroom.Id);
                    }
                }
            }
            else if (delivery.ClientDeliveries != null && delivery.ClientDeliveries.Count > 0)
            {
                delivery.Delivered = DeliveryStatusEnum.Waiting;
                foreach (var client in delivery.ClientDeliveries)
                {
                    client.DateUpdated = DateTime.Now;
                    //  Change from Pending to Waiting
                    client.DeliveryStatus = DeliveryStatusEnum.Waiting;
                    client.ReleaseStatus = ReleaseStatusEnum.Waiting;

                    _context.STClientDeliveries.Update(client);
                    _context.SaveChanges();

                    if (order != null)
                    {
                        if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                        {
                            this.UpdateSTStock(client.Id, false);
                        }
                        else
                        {
                            ////added for ticket #328 moved creation of ststock
                            //if(order.DeliveryType == DeliveryTypeEnum.Delivery && order.TransactionType == TransactionTypeEnum.PO && order.OrderType == OrderTypeEnum.ClientOrder)
                            //{
                            //    this.AddStStock(client, order);
                            //}
                            this.UpdateWHStock(client.Id, false);
                        }
                    }
                    else
                    {
                        this.UpdateWHStock(client.Id, false);
                    }
                }
            }


            _context.STDeliveries.Update(delivery);
            _context.SaveChanges();
        }

        public void UpdateDeliveryStatus(ISTStockService stockService, STDelivery param)
        {
            var delivery = _context.STDeliveries
                .Include(p => p.ClientDeliveries)
                                .Where(x => x.Id == param.Id)
                            .SingleOrDefault();

            var order = _context.STOrders.Where(p => p.Id == delivery.STOrderId).FirstOrDefault();


            if(order?.OrderType == OrderTypeEnum.ClientOrder && order?.DeliveryType == DeliveryTypeEnum.Delivery && order?.TransactionType == TransactionTypeEnum.PO)
            {
                foreach (var client in delivery.ClientDeliveries)
                {
                    var incomingStock = new STStock
                    {
                        STClientDeliveryId = client.Id,
                        StoreId = order.StoreId,
                        ItemId = client.ItemId,
                        OnHand = client.Quantity,
                        DeliveryStatus = DeliveryStatusEnum.Delivered,
                        ReleaseStatus = ReleaseStatusEnum.Released
                    };

                    stockService.InsertSTStock(incomingStock);
                }


                var sales = _context.STSales.Where(p => p.STOrderId == delivery.STOrderId && p.StoreId == delivery.StoreId && p.STDeliveryId == delivery.Id).Include(p => p.SoldItems).FirstOrDefault();
                if(sales != null)
                {
                    //added for ticket #632
                    sales.DateUpdated = DateTime.Now;
                    _context.STSales.Update(sales);
                    
                    foreach (var sold in sales.SoldItems)
                    {
                        var stStock = new STStock
                        {
                            STSalesDetailId = sold.Id,
                            StoreId = order.StoreId,
                            ItemId = sold.ItemId,
                            OnHand = -sold.Quantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered,
                            ReleaseStatus = ReleaseStatusEnum.Released,
                        };

                        stockService.InsertSTStock(stStock);
                    }

                }
               
            }

            if (order?.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
            {
                // Create Sales for Requestor
                //  Add sales record
                var sales = new STSales
                {
                    STOrderId = delivery.STOrderId,
                    StoreId = delivery.StoreId,
                    OrderToStoreId = order.OrderToStoreId,
                    SINumber = delivery.SINumber,
                    DRNumber = delivery.DRNumber,
                    ORNumber = delivery.ORNumber,
                    ClientName = delivery.ClientName,
                    ContactNumber = delivery.ContactNumber,
                    Address1 = delivery.Address1,
                    Address2 = delivery.Address2,
                    Address3 = delivery.Address3,
                    DeliveryType = order.DeliveryType,
                    SoldItems = new List<STSalesDetail>(),
                    ReleaseDate = DateTime.Now
                };

                sales.SalesType = new STSalesService(_context).GetSalesType(order);

                //GetSt Client Delivery
                var clientDelivery = _context.STClientDeliveries
                                            .Where(p => p.STDeliveryId == delivery.Id).ToList();

                if (clientDelivery != null)
                {
                    #region Requestor [Stock History and Sales]

                    for (int i = 0; i < clientDelivery.Count(); i++)
                    {
                        // Sales Detail
                        var salesDetail = new STSalesDetail
                        {
                            ItemId = clientDelivery[i].ItemId,
                            Quantity = clientDelivery[i].Quantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered
                        };

                        sales.SoldItems.Add(salesDetail);

                        // Add Stock History For Requestor upon setting Mark as Delivered
                        var objSTStock = new STStock
                        {
                            STClientDeliveryId = clientDelivery[i].Id,
                            StoreId = order.StoreId,
                            ItemId = clientDelivery[i].ItemId,
                            OnHand = clientDelivery[i].Quantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered,
                        };
                        stockService.InsertSTStock(objSTStock);

                        //moved outside for loop
                        //if (sales.SoldItems.Count > 0)
                        //{
                        //    new STSalesService(_context).InsertSales(sales);
                        //    //removed for ticket #328
                        //    foreach (var sold in sales.SoldItems)
                        //    {
                        //        var stStock = new STStock
                        //        {
                        //            STSalesDetailId = sold.Id,
                        //            StoreId = order.StoreId,
                        //            ItemId = sold.ItemId,
                        //            OnHand = -sold.Quantity,
                        //            DeliveryStatus = DeliveryStatusEnum.Delivered,
                        //            ReleaseStatus = ReleaseStatusEnum.Released,
                        //        };

                        //        stockService.InsertSTStock(stStock);
                        //    }
                        //}

                        #endregion


                        #region Requestee [Stock History]

                        //Get stocks of  Requestee and update
                        var stocks = _context.STStocks
                                                   .Where(p => p.STClientDeliveryId == clientDelivery[i].Id
                                                   && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                   && p.ReleaseStatus == ReleaseStatusEnum.Released
                                                   && p.ItemId == clientDelivery[i].ItemId).FirstOrDefault();

                        if(stocks == null)
                        {
                            continue;
                        }

                        stocks.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        stocks.DateUpdated = DateTime.Now;

                        _context.STStocks.Update(stocks);

                        #endregion







                    }

                    if (sales.SoldItems.Count > 0)
                    {
                        new STSalesService(_context).InsertSales(sales);
                        //removed for ticket #328
                        foreach (var sold in sales.SoldItems)
                        {
                            var stStock = new STStock
                            {
                                STSalesDetailId = sold.Id,
                                StoreId = order.StoreId,
                                ItemId = sold.ItemId,
                                OnHand = -sold.Quantity,
                                DeliveryStatus = DeliveryStatusEnum.Delivered,
                                ReleaseStatus = ReleaseStatusEnum.Released,
                            };

                            stockService.InsertSTStock(stStock);
                        }
                    }
                }
            }

          

            delivery.Delivered = DeliveryStatusEnum.Delivered;
            delivery.DateUpdated = DateTime.Now;


            _context.STDeliveries.Update(delivery);
            _context.SaveChanges();

          

            //added for optimization added field on storder orderstatus will check if the order is completed
              if (order?.OrderType != OrderTypeEnum.ShowroomStockOrder && order?.DeliveryType == DeliveryTypeEnum.Delivery)
              {

                var stord = _context.STOrders.Where(p => p.Id == delivery.STOrderId)
                .Include(p => p.OrderedItems)
                .Include(p => p.Deliveries)
                .ThenInclude(c => c.ClientDeliveries)
                .FirstOrDefault();


                var clientDelId = stord.Deliveries.SelectMany(p => p.ClientDeliveries.Select(x => x.Id)).ToArray();



                var deliveredQty = _context.STStocks.Where(p => clientDelId.Contains((int)p.STClientDeliveryId) && p.OnHand > 0
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered &&
                                                             (stord.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder 
                                                             ? p.ReleaseStatus == null 
                                                             : p.ReleaseStatus == ReleaseStatusEnum.Released))
                                                             .Sum(p => p.OnHand);


                var approvedQty = stord.OrderedItems.Sum(p => p.ApprovedQuantity);



                if (approvedQty == deliveredQty)
                {
                    stord.OrderStatus = OrderStatusEnum.Completed;
                    stord.DateUpdated = DateTime.Now;
                    _context.STOrders.Update(stord);
                    _context.SaveChanges();

                }

            }

              



        


          

            if (delivery.STSalesId != null)
            {
                var sales = _context.STSales.Where(p => p.Id == delivery.STSalesId && p.StoreId == delivery.StoreId)
                                    .Include(p => p.SoldItems).FirstOrDefault();

                var stDel = _context.STDeliveries
                                    .Include(p => p.ClientDeliveries)
                                    .Where(p => p.Delivered == DeliveryStatusEnum.Delivered &&
                                                p.STSalesId == delivery.STSalesId);
                
                if(stDel != null)
                {
                    int? delQuantity = 0;
                    foreach (var cDel in stDel)
                    {
                        delQuantity += Convert.ToInt32(
                                                        cDel.ClientDeliveries
                                                        .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                 && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                        .Sum(p => p.Quantity)
                                                        );
                    }

                    var soldQty = sales.SoldItems.Sum(p => p.Quantity);

                    if ((soldQty == delQuantity) && delQuantity != 0)
                    {
                        
                        //added for optimization
                        sales.OrderStatus = OrderStatusEnum.Completed;

                        sales.DateUpdated = DateTime.Now;
                        _context.STSales.Update(sales);
                        _context.SaveChanges();
                    }
                }
               

                
            }
           
        }



        private void UpdateWHStock(int? id, bool isShowroomDelivery = true)
        {
            WHStock whstock = null;
            if (isShowroomDelivery)
            {
                whstock = _context.WHStocks.Where(p => p.STShowroomDeliveryId == id).FirstOrDefault();
            }
            else
            {
                whstock = _context.WHStocks.Where(p => p.STClientDeliveryId == id).FirstOrDefault();
            }

            if (whstock != null)
            {
                //  Change from Pending to Waiting
                whstock.DeliveryStatus = DeliveryStatusEnum.Waiting;

                //  Set ReleaseStatus to Waiting
                whstock.ReleaseStatus = ReleaseStatusEnum.Waiting;
                whstock.DateUpdated = DateTime.Now;

                _context.WHStocks.Update(whstock);
                _context.SaveChanges();
            }
        }

        private void UpdateSTStock(int? id, bool isShowroomDelivery = true)
        {
            STStock ststock = null;
            if (isShowroomDelivery)
            {
                ststock = _context.STStocks.Where(p => p.STShowroomDeliveryId == id).FirstOrDefault();
            }
            else
            {
                ststock = _context.STStocks.Where(p => p.STClientDeliveryId == id).FirstOrDefault();
            }

            if (ststock != null)
            {
                //  Change from Pending to Waiting
                ststock.DeliveryStatus = DeliveryStatusEnum.Waiting;

                //  Set ReleaseStatus to Waiting
                ststock.ReleaseStatus = ReleaseStatusEnum.Waiting;
                ststock.DateUpdated = DateTime.Now;

                _context.STStocks.Update(ststock);
                _context.SaveChanges();
            }
        }

        private void AddStStock(STClientDelivery client, STOrder order)
        {
            var stockService = new STStockService(_context);
            var stStock = new STStock
            {
                STClientDeliveryId = client.Id,
                StoreId = order.StoreId,
                ItemId = client.ItemId,
                OnHand = client.Quantity,
                DeliveryStatus = DeliveryStatusEnum.Waiting,
                ReleaseStatus = ReleaseStatusEnum.Waiting
            };

            stockService.InsertSTStock(stStock);
        }

        public void InsertToSTStock(SaveReceiveItem param, ISTStockService service,ISTOrderService stservice)
        {

            var STOrderId = param.ShowroomDeliveries.Select(p => p.STOrderId).FirstOrDefault();

            var objOrder = _context.STOrders
                                   .Include(p => p.OrderedItems).Where(p => p.Id == STOrderId).FirstOrDefault();

            var createStDel = true;

            var stdelivery = new STDelivery();
            var ShowroomDeliveries = new List<STShowroomDelivery>();

            foreach (var item in param.ShowroomDeliveries)
            {
                //  Get record from STShowroomDeliveries
                var showroom = _context.STShowroomDeliveries.Where(p => p.Id == item.Id && p.ItemId == item.ItemId && p.STDeliveryId == item.STDeliveryId && p.STOrderDetailId == item.STOrderDetailId).FirstOrDefault();

                //  Get record from WHStocks
                var whStock = _context.WHStocks.Where(p => p.STShowroomDeliveryId == item.Id && p.ItemId == item.ItemId).FirstOrDefault();

                var stStock = _context.STStocks
                                      .Where(p => p.StoreId == objOrder.OrderToStoreId
                                                  && p.STShowroomDeliveryId == item.Id
                                                  && p.ItemId == item.ItemId).FirstOrDefault();

                //  Get record from STOrderDetails
                var orderDetail = _context.STOrderDetails.Where(p => p.Id == item.STOrderDetailId && p.ItemId == item.ItemId).FirstOrDefault();
                var isVendor = false;
                if(objOrder.Warehouse != null)
                {
                     isVendor = objOrder.Warehouse.Vendor;

                }

                

                //if (showroom != null && orderDetail != null && ((!objOrder.Warehouse.Vendor && (whStock != null || stStock != null)) || (objOrder.Warehouse.Vendor)))
                if (showroom != null && orderDetail != null && ((!isVendor && (whStock != null || stStock != null)) || (isVendor || showroom.IsRemainingForReceiving)))
                {
                    if (item.DeliveredQuantity == 0)
                    {
                        showroom.DeliveryStatus = DeliveryStatusEnum.NotDelivered;
                        //added for ticket #430
                        if (!isVendor && !showroom.IsRemainingForReceiving)
                        {
                            //only applies for Tile orders
                            if (objOrder.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder)
                            {
                                whStock.DeliveryStatus = DeliveryStatusEnum.NotDelivered;
                                whStock.DateUpdated = DateTime.Now;
                                _context.WHStocks.Update(whStock);
                            }
                            else
                            {
                                stStock.DeliveryStatus = DeliveryStatusEnum.NotDelivered;
                                stStock.DateUpdated = DateTime.Now;
                                _context.STStocks.Update(stStock);
                            }
                        }
                    }
                    else
                    {
                        showroom.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        if (!isVendor && !showroom.IsRemainingForReceiving)
                        {
                            if (objOrder.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder)
                            {
                                //  Mark record as delivered (WHStock)
                                whStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                                whStock.DateUpdated = DateTime.Now;
                                _context.WHStocks.Update(whStock);
                            }
                            else
                            {
                                //  Mark record as delivered (STStock)
                                stStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                                stStock.DateUpdated = DateTime.Now;
                                _context.STStocks.Update(stStock);
                            }
                        }
                    }

                    showroom.DeliveredQuantity = item.DeliveredQuantity;
                    showroom.Remarks = item.Remarks;

                    showroom.DateUpdated = DateTime.Now;

                    _context.STShowroomDeliveries.Update(showroom);
                    _context.SaveChanges();


                    //  Get all total delivered item in STShowroomDelivery by STOrderDetail.Id
                    var totalDelivered = Convert.ToInt32(
                                            _context.STShowroomDeliveries
                                            .Where(p => p.STOrderDetailId == item.STOrderDetailId
                                                    && p.DeliveryStatus == DeliveryStatusEnum.Delivered)
                                            .Sum(p => p.DeliveredQuantity)
                                        );

                    //  Check if approved quantity in STOrderDetail is equal to totalDelivered
                    if (orderDetail.ApprovedQuantity == totalDelivered)
                    {


                        //  Mark record as delivered (STOrderDetail)
                        orderDetail.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        orderDetail.ReleaseStatus = ReleaseStatusEnum.Released;
                        orderDetail.DateReleased = DateTime.Now;
                        orderDetail.DateUpdated = DateTime.Now;
                        _context.STOrderDetails.Update(orderDetail);

                        _context.SaveChanges();
                    }
                    //remove partial receiving of client PO and showroom order
                    else if(isVendor /*|| ((objOrder.DeliveryType == DeliveryTypeEnum.ShowroomPickup || objOrder.DeliveryType == DeliveryTypeEnum.Delivery))*/){
                        //check if approved quantity is less than the delivered quantity
                        if ((showroom.DeliveredQuantity < orderDetail.ApprovedQuantity) && (totalDelivered < orderDetail.ApprovedQuantity))
                        {

                            if (((showroom.DeliveredQuantity < showroom.Quantity) && (objOrder.DeliveryType == DeliveryTypeEnum.ShowroomPickup || objOrder.DeliveryType == DeliveryTypeEnum.Delivery)))
                            {
                                if(createStDel)
                                {

                                    stdelivery.STOrderId = objOrder.Id;
                                    stdelivery.Id = 0;
                                    stdelivery.DateCreated = DateTime.Now;
                                    stdelivery.DeliveryDate = DateTime.Now;
                                    stdelivery.ApprovedDeliveryDate = DateTime.Now;
                                    stdelivery.StoreId = objOrder.StoreId.Value;
                                    if(objOrder.OrderType == OrderTypeEnum.ClientOrder)
                                    {
                                        stdelivery.DRNumber = objOrder.WHDRNumber;
                                    }
                                    if(objOrder.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                    {
                                        stdelivery.DeliveryFromStoreId = objOrder.OrderToStoreId;
                                        var sales = _context.STSales.Where(p => p.STOrderId == objOrder.Id).FirstOrDefault();
                                        if (sales != null)
                                        {
                                            stdelivery.STSalesId = sales.Id;
                                        }
                                    }
                                    
                                    stdelivery.Remarks = "Remaining Items for receiving";
                                    stdelivery.IsRemainingForReceivingDelivery = true;

                                }

                                var showroomDelivery = new STShowroomDelivery();

                                showroomDelivery.STOrderDetailId = orderDetail.Id;
                                showroomDelivery.Id = 0;
                                showroomDelivery.DateCreated = DateTime.Now;
                                showroomDelivery.DeliveryStatus = DeliveryStatusEnum.Waiting;
                                showroomDelivery.ReleaseStatus = ReleaseStatusEnum.Released;
                                showroomDelivery.ItemId = orderDetail.ItemId;
                                showroomDelivery.Quantity = showroom.Quantity - showroom.DeliveredQuantity;
                                showroomDelivery.IsRemainingForReceiving = true;
                           
                                //showroomDelivery.Remarks = detail.RequestedRemarks;

                                _context.STShowroomDeliveries.Add(showroomDelivery);
                                _context.SaveChanges();

                                ShowroomDeliveries.Add(showroomDelivery);

                                if(createStDel)
                                {
                                    stdelivery.ShowroomDeliveries = ShowroomDeliveries;
                                    _context.STDeliveries.Add(stdelivery);
                                    _context.SaveChanges();
                                    createStDel = false;
                                }

                            }
                            else
                            {
                                if(isVendor)
                                {
                                    stservice.InsertDeliveryToShowroom(objOrder, objOrder.StoreId.Value);
                                }
                                
                            }
                            
                        }
                                
                    
                        
                    }

                    //  If delivered quantity is more than 0
                    //  Save recordto STSTocks
                    if (item.DeliveredQuantity > 0)
                    {
                        var objSTStock = new STStock
                        {
                            StoreId = objOrder.StoreId,
                            STShowroomDeliveryId = item.Id,
                            ItemId = item.ItemId,
                            OnHand = item.DeliveredQuantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered
                        };

                        service.InsertSTStock(objSTStock);
                    }

                }

                //For PO for Sakrete and Stonepro
                if (isVendor)
                {
                    var delivery = _context.STDeliveries.Where(w => w.Id == showroom.STDeliveryId.Value).FirstOrDefault();
                    delivery.DRNumber = param.DRNumber;
                    delivery.DeliveryDate = param.DeliveryDate;

  
                }

            }


            if (objOrder != null)
            {
                //  Check if order is for client and the delivery type is showroom pickup
                if ((objOrder.OrderType == OrderTypeEnum.ClientOrder || objOrder.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder) && objOrder.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                {

                    var sales = new STSales
                    {
                        STOrderId = objOrder.Id,
                        StoreId = objOrder.StoreId,
                        ClientName = objOrder.ClientName,
                        ContactNumber = objOrder.ContactNumber,
                        Address1 = objOrder.Address1,
                        Address2 = objOrder.Address2,
                        Address3 = objOrder.Address3,
                        SoldItems = new List<STSalesDetail>(),
                        DeliveryType = objOrder.DeliveryType,
                        ReleaseDate = DateTime.Now
                    };

                    if (objOrder.OrderType == OrderTypeEnum.ClientOrder)
                    {
                        sales.SalesType = SalesTypeEnum.ClientOrder;
                        // Added for ticket #632
                        sales.ReleaseDate = null;
                        
                    }
                    else
                    {
                        sales.SalesType = new STSalesService(_context).GetSalesType(objOrder);
                    }


                    foreach (var delivery in param.ShowroomDeliveries)
                    {
                        var showroom = _context.STShowroomDeliveries.Where(p => p.Id == delivery.Id && p.ItemId == delivery.ItemId && p.STDeliveryId == delivery.STDeliveryId && p.STOrderDetailId == delivery.STOrderDetailId).FirstOrDefault();
                        if (showroom != null)
                        {
                            var salesDetail = new STSalesDetail
                            {
                                ItemId = delivery.ItemId,
                                Quantity = delivery.DeliveredQuantity,
                                DeliveryStatus = DeliveryStatusEnum.Waiting
                            };

                            sales.SoldItems.Add(salesDetail);
                        }
                    }

                    if (sales.SoldItems.Count > 0)
                    {
                        new STSalesService(_context).InsertSales(sales);
                    }


                    foreach (var item in sales.SoldItems)
                    {
                        //  Deduct item from store's inventory
                        var stStock = new STStock
                        {
                            STSalesDetailId = item.Id,
                            StoreId = objOrder.StoreId,
                            ItemId = item.ItemId,
                            OnHand = -item.Quantity,
                            DeliveryStatus = DeliveryStatusEnum.Waiting,

                            // #116297
                            //ReleaseStatus = ReleaseStatusEnum.Waiting
                        };

                        service.InsertSTStock(stStock);
                    }
                }
            }

            var objSTOrder = _context.STOrders
                                     .Include(p => p.OrderedItems)
                                     .Where(p => p.Id == objOrder.Id)
                                     .FirstOrDefault();
          


            if (objSTOrder.OrderedItems != null && objSTOrder.OrderedItems.Count > 0)
            {
                //  Check if all ordered items are delivered and released
                if (objSTOrder.OrderedItems.Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                    && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                        .Count() == 0)
                {
                    objSTOrder.ReleaseDate = DateTime.Now;
                    objSTOrder.DateUpdated = DateTime.Now;
                  

                    if (objSTOrder.DeliveryType == DeliveryTypeEnum.ShowroomPickup || objSTOrder.DeliveryType == DeliveryTypeEnum.Delivery)
                    {
                        var deliveredOrder = objSTOrder.OrderedItems
                                        .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                        && p.ReleaseStatus == ReleaseStatusEnum.Released).Count();

                        var orderedItemCount = objSTOrder.OrderedItems.Count();

                        if (deliveredOrder == orderedItemCount)
                        {
                            objSTOrder.OrderStatus = OrderStatusEnum.Completed;
                        }
                       
                    }
                   

                    _context.STOrders.Update(objSTOrder);
                    _context.SaveChanges();
                }

            }

        }

        public void InsertDeliveryForClient(STDelivery stdelivery)
        {
            var order = _context.STOrders.Where(p => p.Id == stdelivery.Id).FirstOrDefault();
            if (order != null)
            {
                stdelivery.STOrderId = stdelivery.Id;
                stdelivery.Id = 0;
                stdelivery.DateCreated = DateTime.Now;

                if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {
                    stdelivery.StoreId = order.StoreId;
                    stdelivery.DeliveryFromStoreId = order.OrderToStoreId;
                }

                if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {
                    var sales = _context.STSales.Where(p => p.STOrderId == order.Id).FirstOrDefault();
                    if (sales != null)
                    {
                        stdelivery.STSalesId = sales.Id;
                    }
                }

                foreach (var delivery in stdelivery.ClientDeliveries)
                {

                    //  Check if request deliver quantity is 0
                    if (delivery.Quantity == 0)
                    {
                        //  Skip record
                        continue;
                    }

                    delivery.Id = 0;
                    delivery.DateCreated = DateTime.Now;
                    delivery.DeliveryStatus = DeliveryStatusEnum.Pending;
                    delivery.ReleaseStatus = ReleaseStatusEnum.Pending;
                    _context.STClientDeliveries.Add(delivery);
                    _context.SaveChanges();
                }

                //  Filter only record with request deliver quantity more than 0
                stdelivery.ClientDeliveries = stdelivery.ClientDeliveries.Where(p => p.Quantity > 0).ToList();

                if (order.DeliveryType == DeliveryTypeEnum.Delivery)
                {
                    stdelivery.Delivered = DeliveryStatusEnum.Pending;
                }

                _context.STDeliveries.Add(stdelivery);
                _context.SaveChanges();
            }


            if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
            {
                var stStockService = new STStockService(_context);

                foreach (var delivery in stdelivery.ClientDeliveries)
                {
                    var ststock = new STStock
                    {
                        StoreId = order.OrderToStoreId,
                        STClientDeliveryId = delivery.Id,
                        ItemId = delivery.ItemId,
                        OnHand = -delivery.Quantity,
                        DeliveryStatus = DeliveryStatusEnum.Pending,
                        ReleaseStatus = ReleaseStatusEnum.Pending
                    };

                    stStockService.InsertSTStock(ststock);
                }
            }
            else
            {
                var whStockService = new WHStockService(_context);

                foreach (var delivery in stdelivery.ClientDeliveries)
                {
                    var whstock = new WHStock
                    {
                        WarehouseId = order.WarehouseId,
                        STClientDeliveryId = delivery.Id,
                        ItemId = delivery.ItemId,
                        OnHand = -delivery.Quantity,
                        TransactionType = TransactionTypeEnum.PO,
                        DeliveryStatus = DeliveryStatusEnum.Pending,
                        ReleaseStatus = ReleaseStatusEnum.Pending
                    };

                    whStockService.InsertStock(whstock);

                    if (order.DeliveryType == DeliveryTypeEnum.Pickup && order.OrderType == OrderTypeEnum.ClientOrder)
                    {
                        var del = new STDelivery();
                        del.Id = (int)delivery.STDeliveryId;
                        del.DriverName = "";
                        del.PlateNumber = "";
                        del.ApprovedDeliveryDate = stdelivery.DeliveryDate;

                        UpdateDelivery(del);
                    }
                }
            }
        }

        public object GetAllDeliveriesForShowroom(SearchDeliveries search, IMapper mapper, AppSettings appSettings)
        {
            IQueryable<STDelivery> list = _context.STDeliveries
                                                .Include(p => p.Order)
                                                    .ThenInclude(p => p.Warehouse)
                                                .Include(p => p.Order)
                                                    .ThenInclude(p => p.Store)
                                                .Include(p => p.ShowroomDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                  .Where(p => p.STOrderId.HasValue && ((p.Order != null) 
                                                  ? ((p.Order.Warehouse != null && p.Order.Warehouse.Vendor == false) || p.Order.Warehouse == null) 
                                                  : p.STOrderId.HasValue));

            list = list.Where(p => p.ShowroomDeliveries.Count > 0);

            if(!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                list = list.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                list = list.Where(y => search.DeliveryDateFrom <= y.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                list = list.Where(y => search.DeliveryDateTo >= y.DeliveryDate);
            }

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                list = list.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (search.DeliveryType.HasValue)
            {
                list = list.Where(p => p.Order.DeliveryType == search.DeliveryType);
            }

            var deliveryList = new List<STDelivery>();


            if (search.DeliveryStatus != null && search.DeliveryStatus.Count() > 0)
            {

                var filteredList = list.Where(p => (p.Delivered != null && search.DeliveryStatus.Contains(p.Delivered)));
                deliveryList = list.Where(p => (p.Delivered == null && p.ShowroomDeliveries.Any(c => search.DeliveryStatus.Contains(c.DeliveryStatus)))).ToList();

                if (search.DeliveryStatus.Contains(DeliveryStatusEnum.WaitingForConfirmation))
                {

                    var ere = list.Where(p => ((p.Order.OrderType == OrderTypeEnum.ClientOrder || p.Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    && (p.Delivered == DeliveryStatusEnum.Waiting || p.Delivered == DeliveryStatusEnum.Delivered) 
                    && p.Order.DeliveryType == DeliveryTypeEnum.Delivery));
                   
                }
                deliveryList.AddRange(filteredList);
            }
            if(deliveryList.Count == 0)
            {
                deliveryList = list.ToList();
            }

    


            GetAllResponse response = null; 
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(deliveryList.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                deliveryList = deliveryList.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(deliveryList.Count());
            }

            // converting StDelivery to StdeliveryDTO for display
            var deliveries = new List<STDeliveryDTO>();

            deliveries = Mapper.Map(deliveryList, deliveries);

            var retList = new List<object>();

            var stores = _context.Stores.AsNoTracking();
            for (int i = 0; i < deliveries.Count(); i++)
            {
                var IsInterBranch = false;
                var IsTransferShowroomPickup = false;
                var DisplayWHDRNumber = true;
                var TransferHeader = "";

                var order = deliveries[i].Order;
                if (order != null)
                {
                    if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {
                        var storeCompany = stores.Where(p => p.Id == order.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                        var orderToStoreCompany = stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                        // Returns true or false
                        IsInterBranch = (storeCompany == orderToStoreCompany);

                        // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                        TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                        IsTransferShowroomPickup = (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup);

                        //if intercompany and delivery type = showroom pickup  added for ticket #41
                        if (order.DeliveryType == DeliveryTypeEnum.Delivery || (IsInterBranch == false && order.DeliveryType == DeliveryTypeEnum.ShowroomPickup))
                        {
                            DisplayWHDRNumber = false;
                        }
                    }
                }

                DeliveryStatusEnum? DeliveryStatus = (deliveries[i].ShowroomDeliveries != null && deliveries[i].ShowroomDeliveries.Count() > 0)
                                    ? deliveries[i].ShowroomDeliveries.Where(p => p.STDeliveryId == deliveries[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                    : null;

                var DeliveryStatusStr = (DeliveryStatus != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), DeliveryStatus)) : null;


                var obj = new
                {
                    deliveries[i].Id,
                    deliveries[i].STOrderId,
                    TransactionNo = (deliveries[i].Order != null) ? deliveries[i].Order.TransactionNo : _context.STSales.Where(p => p.Id == deliveries[i].STSalesId).Select(p => p.TransactionNo).FirstOrDefault(),
                    TransactionType = (deliveries[i].Order != null) ? deliveries[i].Order.TransactionType : null,
                    TransactionTypeStr = (deliveries[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), deliveries[i].Order.TransactionType)) : null,
                    OrderType = (deliveries[i].Order != null) ? deliveries[i].Order.OrderType : null,
                    OrderTypeStr = (deliveries[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), deliveries[i].Order.OrderType)) : null,
                    deliveries[i].DRNumber,
                    DeliveryRemarks = deliveries[i].Remarks,
                    deliveries[i].DeliveryDate,
                    ORNumber = (deliveries[i].Order != null) ? deliveries[i].Order.ORNumber : null,
                    WhDrNumber = (deliveries[i].Order != null) ? deliveries[i].Order.WHDRNumber : null,

                    OrderedBy = (deliveries[i].Order != null)
                                            ? (
                                                deliveries[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? (deliveries[i].Order.DeliveryType == DeliveryTypeEnum.Delivery ? deliveries[i].ClientName
                                                : stores.Where(y => y.Id == deliveries[i].Order.StoreId).Select(z => z.Name).FirstOrDefault())
                                                : deliveries[i].Order.Store.Name
                                            )
                                            : null,
                    OrderedByStore = (deliveries[i].Order != null)
                                            ? (
                                                deliveries[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? _context.Stores.Where(y => y.Id == deliveries[i].Order.StoreId).Select(z => z.Name).FirstOrDefault()
                                                : deliveries[i].Order.Store.Name
                                            )
                                            : null,

                    OrderedByAddress = (deliveries[i].Order != null) ? (deliveries[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && deliveries[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                              ? deliveries[i].Address1 + " " + deliveries[i].Address2 + " " + deliveries[i].Address3
                              : deliveries[i].Order.Store.Address) : null,
                    OrderedByContact = (deliveries[i].Order != null) ? (deliveries[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && deliveries[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ? deliveries[i].ContactNumber
                                                                    : deliveries[i].Order.Store.ContactNumber)
                                                                    : null,
                    OrderedTo = (deliveries[i].Order != null)
                                            ? (
                                                    (deliveries[i].Order.Warehouse != null)
                                                    ? deliveries[i].Order.Warehouse.Name
                                                    :
                                                        (deliveries[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                        ? stores.Where(y => y.Id == deliveries[i].Order.OrderToStoreId).Select(z => z.Name).FirstOrDefault()
                                                        : null
                                               )
                                            : deliveries[i].Store.Name,
                    DeliveryType = (deliveries[i].Order != null) ? deliveries[i].Order.DeliveryType : DeliveryTypeEnum.Delivery,
                    DeliveryTypeStr = (deliveries[i].Order != null) ? deliveries[i].Order.DeliveryTypeStr : EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), DeliveryTypeEnum.Delivery)),
                    ClientPoSP = (deliveries[i].Order != null) ?
                                        deliveries[i].Order.DeliveryType == DeliveryTypeEnum.ShowroomPickup && deliveries[i].Order.OrderType == OrderTypeEnum.ClientOrder ? true : false
                                        : false,
                    deliveries[i].ApprovedDeliveryDate,
                    DeliveryQty = deliveries[i].ShowroomDeliveries.Sum(p => p.Quantity),
                    PODate = (deliveries[i].Order != null) ? deliveries[i].Order.PODate : null,
                    PONumber = (deliveries[i].Order != null) ? deliveries[i].Order.PONumber : null,

                    RequestStatus = (deliveries[i].Order != null) ? deliveries[i].Order.RequestStatus : null,
                    RequestStatusStr = (deliveries[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), deliveries[i].Order.RequestStatus)) : null,
                    deliveries[i].ClientName,
                    PreferredTimeStr = (deliveries[i].PreferredTime != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(PreferredTimeEnum), deliveries[i].PreferredTime)) : null,
                    deliveries[i].Address1,
                    deliveries[i].Address2,
                    deliveries[i].Address3,
                    deliveries[i].ContactNumber,
                    deliveries[i].DriverName,
                    deliveries[i].PlateNumber,
                    DisplayWHDRNumber,
                    Remarks = (deliveries[i].Order != null) ? deliveries[i].Order.Remarks : null,
                    Deliveries = deliveries[i].ShowroomDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                }),
                    DeliveryStatus,
                    DeliveryStatusStr,
                    deliveries[i].Delivered,
                    DeliveredStr = (deliveries[i].Delivered != null) ? this.GetDeliveryStatusStr(deliveries[i].Delivered) : null,
                    IsCustomDelivery = (deliveries[i].Delivered != null) ? true : false,
                    IsUpdatable = (deliveries[i].Delivered == null || deliveries[i].Delivered == DeliveryStatusEnum.Delivered) ? false : (deliveries[i].Delivered == DeliveryStatusEnum.Waiting) ? true : false,
                    ShowDeliveredButton = (deliveries[i].Order != null)
                                            ? (((deliveries[i].Order.OrderType == OrderTypeEnum.ClientOrder || deliveries[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder) && (deliveries[i].Delivered == DeliveryStatusEnum.Waiting || deliveries[i].Delivered == DeliveryStatusEnum.Delivered))
                                                    && deliveries[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClient = (deliveries[i].Order != null)
                                            ? (deliveries[i].Order.OrderType == OrderTypeEnum.ClientOrder && deliveries[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClientPickup = (deliveries[i].Order != null)
                                            ? (deliveries[i].Order.OrderType == OrderTypeEnum.ClientOrder && deliveries[i].Order.DeliveryType == DeliveryTypeEnum.Pickup)
                                                ? true : false
                                            : false,
                    IsInterBranch,
                    TransferHeader,
                    IsTransferShowroomPickup
                };

                retList.Add(obj);




            }

            response.List.AddRange(deliveryList);


            return response;

        }

        public object GetAllForDeliveriesPaged(SearchDeliveries search, IMapper mapper, AppSettings appSettings)
        {
            IQueryable<STDelivery> list = _context.STDeliveries
                                            .Include(p => p.Order)
                                                .ThenInclude(p => p.Warehouse)
                                             .Include(p => p.Order)
                                                .ThenInclude(p => p.Store)
                                            .Include(p => p.ShowroomDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Include(p => p.ClientDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Include(p => p.Store)
                                            .Where(p => p.STOrderId.HasValue && ((p.Order != null) ? ((p.Order.Warehouse != null && p.Order.Warehouse.Vendor == false) || p.Order.Warehouse == null) : p.STOrderId.HasValue));


            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                //  Searched by DRNumber
                list = list.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                list = list.Where(y => search.DeliveryDateFrom <= y.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                list = list.Where(y => search.DeliveryDateTo >= y.DeliveryDate);
            }

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                list = list.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (search.DeliveryType.HasValue)
            {
                list = list.Where(p => p.Order.DeliveryType == search.DeliveryType);
            }

            var deliveryList = new List<STDelivery>();
            deliveryList = list.ToList();
            if (search.DeliveryStatus != null && search.DeliveryStatus.Count() > 0)
            {
                var predicate = (search.DeliveryStatus.Count() == 2) ? PredicateBuilder.True<STDelivery>() : PredicateBuilder.False<STDelivery>();

                foreach (var delcri in search.DeliveryStatus)
                {
                    predicate = predicate.Or(p => (p.Delivered != null && p.Delivered == (DeliveryStatusEnum)delcri));
                    predicate = predicate.Or(p => (p.Delivered == null && p.ClientDeliveries.Any(c => c.DeliveryStatus == (DeliveryStatusEnum)delcri)));
                    predicate = predicate.Or(p => (p.Delivered == null && p.ShowroomDeliveries.Any(c => c.DeliveryStatus == (DeliveryStatusEnum)delcri)));


                }



                deliveryList = deliveryList.Where(predicate.Compile()).ToList();
            }


            var deliveries = new List<STDeliveryDTO>();
            
            deliveries = Mapper.Map(deliveryList, deliveries);



            var rec = deliveries.Where(p => p.IsClienOrder == false).OrderByDescending(p => p.Id).ToList();
            var retList = new List<object>();

            var stores = _context.Stores.AsNoTracking().ToList();


            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(rec.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                rec = rec.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(rec.Count());
            }


            for (int i = 0; i < rec.Count(); i++)
            {
                var IsInterBranch = false;
                var IsTransferShowroomPickup = false;
                var DisplayWHDRNumber = true;
                var TransferHeader = "";

                var order = rec[i].Order;
                if (order != null)
                {
                    if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {
                        var storeCompany = stores.Where(p => p.Id == order.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                        var orderToStoreCompany = stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                        // Returns true or false
                        IsInterBranch = (storeCompany == orderToStoreCompany);

                        // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                        TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                        IsTransferShowroomPickup = (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup);

                        //if intercompany and delivery type = showroom pickup  added for ticket #41
                        if (order.DeliveryType == DeliveryTypeEnum.Delivery || (IsInterBranch == false && order.DeliveryType == DeliveryTypeEnum.ShowroomPickup))
                        {
                            DisplayWHDRNumber = false;
                        }
                    }
                }

                DeliveryStatusEnum? DeliveryStatus = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                    ? rec[i].ShowroomDeliveries.Where(p => p.STDeliveryId == rec[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                    : (rec[i].ClientDeliveries != null && rec[i].ClientDeliveries.Count() > 0)
                                        ? rec[i].ClientDeliveries.Where(p => p.STDeliveryId == rec[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                : null;

                var DeliveryStatusStr = (DeliveryStatus != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), DeliveryStatus)) : null;


                var obj = new
                {
                    rec[i].Id,
                    rec[i].STOrderId,
                    TransactionNo = (rec[i].Order != null) ? rec[i].Order.TransactionNo : _context.STSales.Where(p => p.Id == rec[i].STSalesId).Select(p => p.TransactionNo).FirstOrDefault(),
                    TransactionType = (rec[i].Order != null) ? rec[i].Order.TransactionType : null,
                    TransactionTypeStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), rec[i].Order.TransactionType)) : null,
                    OrderType = (rec[i].Order != null) ? rec[i].Order.OrderType : null,
                    OrderTypeStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), rec[i].Order.OrderType)) : null,
                    rec[i].DRNumber,
                    DeliveryRemarks = rec[i].Remarks,
                    rec[i].DeliveryDate,
                    ORNumber = (rec[i].Order != null) ? rec[i].Order.ORNumber : null,
                    WhDrNumber = (rec[i].Order != null) ? rec[i].Order.WHDRNumber : null,

                    OrderedBy = (rec[i].Order != null)
                                            ? (
                                                rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery ? rec[i].ClientName
                                                : _context.Stores.Where(y => y.Id == rec[i].Order.StoreId).Select(z => z.Name).FirstOrDefault())
                                                : rec[i].Order.Store.Name
                                            )
                                            : null,
                    OrderedByStore = (rec[i].Order != null)
                                            ? (
                                                rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? _context.Stores.Where(y => y.Id == rec[i].Order.StoreId).Select(z => z.Name).FirstOrDefault()
                                                : rec[i].Order.Store.Name
                                            )
                                            : null,

                    OrderedByAddress = (rec[i].Order != null) ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                              ? rec[i].Address1 + " " + rec[i].Address2 + " " + rec[i].Address3
                              : rec[i].Order.Store.Address) : null,
                    OrderedByContact = (rec[i].Order != null) ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ? rec[i].ContactNumber
                                                                    : rec[i].Order.Store.ContactNumber)
                                                                    : null,
                    OrderedTo = (rec[i].Order != null)
                                            ? (
                                                    (rec[i].Order.Warehouse != null)
                                                    ? rec[i].Order.Warehouse.Name
                                                    :
                                                        (rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                        ? _context.Stores.Where(y => y.Id == rec[i].Order.OrderToStoreId).Select(z => z.Name).FirstOrDefault()
                                                        : null
                                               )
                                            : rec[i].Store.Name,
                    DeliveryType = (rec[i].Order != null) ? rec[i].Order.DeliveryType : DeliveryTypeEnum.Delivery,
                    DeliveryTypeStr = (rec[i].Order != null) ? rec[i].Order.DeliveryTypeStr : EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), DeliveryTypeEnum.Delivery)),
                    ClientPoSP = (rec[i].Order != null) ?
                                        rec[i].Order.DeliveryType == DeliveryTypeEnum.ShowroomPickup && rec[i].Order.OrderType == OrderTypeEnum.ClientOrder ? true : false
                                        : false,
                    rec[i].ApprovedDeliveryDate,
                    DeliveryQty = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                                ? rec[i].ShowroomDeliveries.Sum(p => p.Quantity)
                                                : rec[i].ClientDeliveries.Sum(p => p.Quantity),
                    PODate = (rec[i].Order != null) ? rec[i].Order.PODate : null,
                    PONumber = (rec[i].Order != null) ? rec[i].Order.PONumber : null,

                    RequestStatus = (rec[i].Order != null) ? rec[i].Order.RequestStatus : null,
                    RequestStatusStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), rec[i].Order.RequestStatus)) : null,
                    rec[i].ClientName,
                    PreferredTimeStr = (rec[i].PreferredTime != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(PreferredTimeEnum), rec[i].PreferredTime)) : null,
                    rec[i].Address1,
                    rec[i].Address2,
                    rec[i].Address3,
                    rec[i].ContactNumber,
                    rec[i].DriverName,
                    rec[i].PlateNumber,
                    DisplayWHDRNumber,
                    Remarks = (rec[i].Order != null) ? rec[i].Order.Remarks : null,
                    Deliveries = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                                ? rec[i].ShowroomDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                })
                                                : rec[i].ClientDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                }),
                    DeliveryStatus,
                    DeliveryStatusStr,
                    rec[i].Delivered,
                    DeliveredStr = (rec[i].Delivered != null) ? this.GetDeliveryStatusStr(rec[i].Delivered) : null,
                    IsCustomDelivery = (rec[i].Delivered != null) ? true : false,
                    IsUpdatable = (rec[i].Delivered == null || rec[i].Delivered == DeliveryStatusEnum.Delivered) ? false : (rec[i].Delivered == DeliveryStatusEnum.Waiting) ? true : false,
                    ShowDeliveredButton = (rec[i].Order != null)
                                            ? (((rec[i].Order.OrderType == OrderTypeEnum.ClientOrder || rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder) && (rec[i].Delivered == DeliveryStatusEnum.Waiting || rec[i].Delivered == DeliveryStatusEnum.Delivered))
                                                    && rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClient = (rec[i].Order != null)
                                            ? (rec[i].Order.OrderType == OrderTypeEnum.ClientOrder && rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClientPickup = (rec[i].Order != null)
                                            ? (rec[i].Order.OrderType == OrderTypeEnum.ClientOrder && rec[i].Order.DeliveryType == DeliveryTypeEnum.Pickup)
                                                ? true : false
                                            : false,
                    IsInterBranch,
                    TransferHeader,
                    IsTransferShowroomPickup
                };
  
                retList.Add(obj);




            }

            response.List.AddRange(retList);

            return response;


        }



        public object GetAllForDeliveriesPaged2(SearchDeliveries search, IMapper mapper, AppSettings appSettings)
        {
            IQueryable<STDelivery> list = _context.STDeliveries
                                            .Include(p => p.Order)
                                                .ThenInclude(p => p.Warehouse)
                                             .Include(p => p.Order)
                                                .ThenInclude(p => p.Store)
                                            .Include(p => p.ShowroomDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Include(p => p.ClientDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Include(p => p.Store)
                                            .Where(p => p.STOrderId.HasValue && ((p.Order != null) ? ((p.Order.Warehouse != null && p.Order.Warehouse.Vendor == false) || p.Order.Warehouse == null) : p.STOrderId.HasValue));


            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                //  Searched by DRNumber
                list = list.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                list = list.Where(y => search.DeliveryDateFrom <= y.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                list = list.Where(y => search.DeliveryDateTo >= y.DeliveryDate);
            }

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                list = list.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (search.DeliveryType.HasValue)
            {
                list = list.Where(p => p.Order.DeliveryType == search.DeliveryType);
            }

            var deliveryList = new List<STDelivery>();


            if (search.DeliveryStatus != null && search.DeliveryStatus.Count() > 0)
            {
                //Will filter base on delivered field
                var filteredList = list.Where(p => (p.Delivered != null && search.DeliveryStatus.Contains(p.Delivered)));

                //Will Get Filtered list for showroom deliveries
                deliveryList = list.Where(p => (p.Delivered == null && p.ShowroomDeliveries.Any(c => search.DeliveryStatus.Contains(c.DeliveryStatus)))).ToList();

                //Will Get Filtered list for client deliveries
                var ClientdeliveryList = list.Where(p => (p.Delivered == null && p.ClientDeliveries.Any(c => search.DeliveryStatus.Contains(c.DeliveryStatus)))).ToList();

                var waitingForConfirmationList = new List<STDelivery>();

                if (search.DeliveryStatus.Contains(DeliveryStatusEnum.WaitingForConfirmation))
                {

                    waitingForConfirmationList = list.Where(p => ((p.Order.OrderType == OrderTypeEnum.ClientOrder || p.Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    && (p.Delivered == DeliveryStatusEnum.Waiting && p.Order.DeliveryType == DeliveryTypeEnum.Delivery)
                    )).ToList();

                    waitingForConfirmationList = waitingForConfirmationList.Where(p => 
                                                                (p.ShowroomDeliveries.Any(x => x.ReleaseStatus == ReleaseStatusEnum.Released) || (p.ClientDeliveries.Any(x => x.ReleaseStatus == ReleaseStatusEnum.Released)))
                                                 ).ToList();

                }

                var existingItem = waitingForConfirmationList.Select(p => p.Id);

                // Will add filtered list
                deliveryList.AddRange(ClientdeliveryList);
                deliveryList.AddRange(filteredList);

                // Will get record with similar id to avoid duplication of records
                var reject = deliveryList.Where(p => existingItem.Contains(p.Id));

                // Will add to delivery except for duplicated records
                deliveryList.AddRange(waitingForConfirmationList.Except(reject));

            }
            if (deliveryList.Count == 0)
            {
                deliveryList = list.ToList();
            }


            var deliveries = new List<STDeliveryDTO>();

            // Converting STdelivery to STdeliveryDTO for display purposes
            deliveries = Mapper.Map(deliveryList, deliveries);



            var rec = deliveries.Where(p => p.IsClienOrder == false).OrderByDescending(p => p.Id).ToList();
            var retList = new List<object>();

            var stores = _context.Stores.AsNoTracking().ToList();


            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(rec.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                rec = rec.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(rec.Count());
            }


            for (int i = 0; i < rec.Count(); i++)
            {
                var IsInterBranch = false;
                var IsTransferShowroomPickup = false;
                var DisplayWHDRNumber = true;
                var TransferHeader = "";

                var order = rec[i].Order;
                if (order != null)
                {
                    if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {
                        var storeCompany = stores.Where(p => p.Id == order.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                        var orderToStoreCompany = stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                        // Returns true or false
                        IsInterBranch = (storeCompany == orderToStoreCompany);

                        // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                        TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                        IsTransferShowroomPickup = (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup);

                        //if intercompany and delivery type = showroom pickup  added for ticket #41
                        if (order.DeliveryType == DeliveryTypeEnum.Delivery || (IsInterBranch == false && order.DeliveryType == DeliveryTypeEnum.ShowroomPickup))
                        {
                            DisplayWHDRNumber = false;
                        }
                    }
                }

                DeliveryStatusEnum? DeliveryStatus = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                    ? rec[i].ShowroomDeliveries.Where(p => p.STDeliveryId == rec[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                    : (rec[i].ClientDeliveries != null && rec[i].ClientDeliveries.Count() > 0)
                                        ? rec[i].ClientDeliveries.Where(p => p.STDeliveryId == rec[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                : null;

                var DeliveryStatusStr = (DeliveryStatus != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), DeliveryStatus)) : null;


                var obj = new
                {
                    rec[i].Id,
                    rec[i].STOrderId,
                    TransactionNo = (rec[i].Order != null) ? rec[i].Order.TransactionNo : _context.STSales.Where(p => p.Id == rec[i].STSalesId).Select(p => p.TransactionNo).FirstOrDefault(),
                    TransactionType = (rec[i].Order != null) ? rec[i].Order.TransactionType : null,
                    TransactionTypeStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), rec[i].Order.TransactionType)) : null,
                    OrderType = (rec[i].Order != null) ? rec[i].Order.OrderType : null,
                    OrderTypeStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), rec[i].Order.OrderType)) : null,
                    rec[i].DRNumber,
                    DeliveryRemarks = rec[i].Remarks,
                    rec[i].DeliveryDate,
                    ORNumber = (rec[i].Order != null) ? rec[i].Order.ORNumber : null,
                    WhDrNumber = (rec[i].Order != null) ? rec[i].Order.WHDRNumber : null,

                    OrderedBy = (rec[i].Order != null)
                                            ? (
                                                rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery ? rec[i].ClientName
                                                : _context.Stores.Where(y => y.Id == rec[i].Order.StoreId).Select(z => z.Name).FirstOrDefault())
                                                : rec[i].Order.Store.Name
                                            )
                                            : null,
                    OrderedByStore = (rec[i].Order != null)
                                            ? (
                                                rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? _context.Stores.Where(y => y.Id == rec[i].Order.StoreId).Select(z => z.Name).FirstOrDefault()
                                                : rec[i].Order.Store.Name
                                            )
                                            : null,

                    OrderedByAddress = (rec[i].Order != null) ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                              ? rec[i].Address1 + " " + rec[i].Address2 + " " + rec[i].Address3
                              : rec[i].Order.Store.Address) : null,
                    OrderedByContact = (rec[i].Order != null) ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ? rec[i].ContactNumber
                                                                    : rec[i].Order.Store.ContactNumber)
                                                                    : null,
                    OrderedTo = (rec[i].Order != null)
                                            ? (
                                                    (rec[i].Order.Warehouse != null)
                                                    ? rec[i].Order.Warehouse.Name
                                                    :
                                                        (rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                        ? _context.Stores.Where(y => y.Id == rec[i].Order.OrderToStoreId).Select(z => z.Name).FirstOrDefault()
                                                        : null
                                               )
                                            : rec[i].Store.Name,
                    DeliveryType = (rec[i].Order != null) ? rec[i].Order.DeliveryType : DeliveryTypeEnum.Delivery,
                    DeliveryTypeStr = (rec[i].Order != null) ? rec[i].Order.DeliveryTypeStr : EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), DeliveryTypeEnum.Delivery)),
                    ClientPoSP = (rec[i].Order != null) ?
                                        rec[i].Order.DeliveryType == DeliveryTypeEnum.ShowroomPickup && rec[i].Order.OrderType == OrderTypeEnum.ClientOrder ? true : false
                                        : false,
                    rec[i].ApprovedDeliveryDate,
                    DeliveryQty = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                                ? rec[i].ShowroomDeliveries.Sum(p => p.Quantity)
                                                : rec[i].ClientDeliveries.Sum(p => p.Quantity),
                    PODate = (rec[i].Order != null) ? rec[i].Order.PODate : null,
                    PONumber = (rec[i].Order != null) ? rec[i].Order.PONumber : null,

                    RequestStatus = (rec[i].Order != null) ? rec[i].Order.RequestStatus : null,
                    RequestStatusStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), rec[i].Order.RequestStatus)) : null,
                    rec[i].ClientName,
                    PreferredTimeStr = (rec[i].PreferredTime != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(PreferredTimeEnum), rec[i].PreferredTime)) : null,
                    rec[i].Address1,
                    rec[i].Address2,
                    rec[i].Address3,
                    rec[i].ContactNumber,
                    rec[i].DriverName,
                    rec[i].PlateNumber,
                    DisplayWHDRNumber,
                    Remarks = (rec[i].Order != null) ? rec[i].Order.Remarks : null,
                    Deliveries = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                                ? rec[i].ShowroomDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity,
                                                    p.Id,
                                                    p.isTonalityAny,
                                                }).OrderBy(p => p.Id)
                                                : rec[i].ClientDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity,
                                                    p.Id,
                                                    p.isTonalityAny
                                                }).OrderBy(p => p.Id),
                    DeliveryStatus,
                    DeliveryStatusStr,
                    rec[i].Delivered,
                    DeliveredStr = (rec[i].Delivered != null) ? this.GetDeliveryStatusStr(rec[i].Delivered) : null,
                    IsCustomDelivery = (rec[i].Delivered != null) ? true : false,
                    IsUpdatable = (rec[i].Delivered == null || rec[i].Delivered == DeliveryStatusEnum.Delivered) ? false : (rec[i].Delivered == DeliveryStatusEnum.Waiting) ? true : false,
                    ShowDeliveredButton = (rec[i].Order != null)
                                            ? (((rec[i].Order.OrderType == OrderTypeEnum.ClientOrder || rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder) && (rec[i].Delivered == DeliveryStatusEnum.Waiting || rec[i].Delivered == DeliveryStatusEnum.Delivered))
                                                    && rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClient = (rec[i].Order != null)
                                            ? (rec[i].Order.OrderType == OrderTypeEnum.ClientOrder && rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClientPickup = (rec[i].Order != null)
                                            ? (rec[i].Order.OrderType == OrderTypeEnum.ClientOrder && rec[i].Order.DeliveryType == DeliveryTypeEnum.Pickup)
                                                ? true : false
                                            : false,
                    IsInterBranch,
                    TransferHeader,
                    IsTransferShowroomPickup
                };

                retList.Add(obj);




            }

            response.List.AddRange(retList);

            return response;


        }


        public IEnumerable<object> GetAllForDeliveries(SearchDeliveries search, IMapper mapper)
        {
            IQueryable<STDelivery> list = _context.STDeliveries
                                            .Include(p => p.ShowroomDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Include(p => p.ClientDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STOrderId.HasValue);


            var deliveries = new List<STDeliveryDTO>();

            foreach (var del in list)
            {
                var mappedDelivery = mapper.Map<STDeliveryDTO>(del);

                var order = _context.STOrders.Where(p => p.Id == del.STOrderId).FirstOrDefault();
                if (order != null)
                {
                    var mappedOrder = mapper.Map<STOrderDTO>(order);
                    mappedDelivery.Order = mappedOrder;


                    var warehouse = _context.Warehouses.Where(p => p.Id == order.WarehouseId).FirstOrDefault();
                    if (warehouse != null)
                    {
                        var mappedWarehouse = mapper.Map<WarehouseDTO>(warehouse);
                        mappedDelivery.Order.Warehouse = mappedWarehouse;
                    }

                    var store = _context.Stores.Where(p => p.Id == del.StoreId).FirstOrDefault();
                    if (store != null)
                    {
                        var mappedStore = mapper.Map<StoreDTO>(store);
                        mappedDelivery.Order.Store = mappedStore;
                    }

                    if ((warehouse != null && warehouse.Vendor == false) || warehouse == null)
                    {
                        deliveries.Add(mappedDelivery);
                    }
                }
                else
                {
                    var store = _context.Stores.Where(p => p.Id == del.StoreId).FirstOrDefault();
                    if (store != null)
                    {
                        var mappedStore = mapper.Map<StoreDTO>(store);
                        mappedDelivery.Store = mappedStore;
                    }
                    deliveries.Add(mappedDelivery);
                }

            }

            var query = deliveries.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                //  Searched by DRNumber
                query = query.Where(y => y.DRNumber?.ToLower() == search.DRNumber.ToLower());
            }

            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                query = query.Where(y => search.DeliveryDateFrom <= y.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                query = query.Where(y => search.DeliveryDateTo >= y.DeliveryDate);
            }

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            var rec = query.OrderByDescending(p => p.Id).ToList();
            var retList = new List<object>();

            var stores = _context.Stores.AsNoTracking().ToList();

            for (int i = 0; i < rec.Count(); i++)
            {
                var IsInterBranch = false;
                var IsTransferShowroomPickup = false;
                var DisplayWHDRNumber = true;
                var TransferHeader = "";

                var order = rec[i].Order;
                if (order != null)
                {
                    if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {
                        var storeCompany = stores.Where(p => p.Id == order.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                        var orderToStoreCompany = stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                        // Returns true or false
                        IsInterBranch = (storeCompany == orderToStoreCompany);

                        // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                        TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                        IsTransferShowroomPickup = (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup);

                        //if intercompany and delivery type = showroom pickup  added for ticket #41
                        if (order.DeliveryType == DeliveryTypeEnum.Delivery || (IsInterBranch == false && order.DeliveryType == DeliveryTypeEnum.ShowroomPickup))
                        {
                            DisplayWHDRNumber = false;
                        }
                    }
                }

                DeliveryStatusEnum? DeliveryStatus = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                    ? rec[i].ShowroomDeliveries.Where(p => p.STDeliveryId == rec[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                    : (rec[i].ClientDeliveries != null && rec[i].ClientDeliveries.Count() > 0)
                                        ? rec[i].ClientDeliveries.Where(p => p.STDeliveryId == rec[i].Id).Select(p => p.DeliveryStatus).FirstOrDefault()
                                : null;

                var DeliveryStatusStr = (DeliveryStatus != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), DeliveryStatus)) : null;

                // Filter by Delivery Status
                if (search.DeliveryStatus != null)
                {
                    if (rec[i].Delivered != null)
                    {
                        if (!search.DeliveryStatus.Contains(rec[i].Delivered))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!search.DeliveryStatus.Contains(DeliveryStatus))
                        {
                            continue;
                        }
                    }
                }

                var obj = new
                {
                    rec[i].Id,
                    rec[i].STOrderId,
                    TransactionNo = (rec[i].Order != null) ? rec[i].Order.TransactionNo : _context.STSales.Where(p => p.Id == rec[i].STSalesId).Select(p => p.TransactionNo).FirstOrDefault(),
                    TransactionType = (rec[i].Order != null) ? rec[i].Order.TransactionType : null,
                    TransactionTypeStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), rec[i].Order.TransactionType)) : null,
                    OrderType = (rec[i].Order != null) ? rec[i].Order.OrderType : null,
                    OrderTypeStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), rec[i].Order.OrderType)) : null,
                    rec[i].DRNumber,
                    DeliveryRemarks = rec[i].Remarks,
                    rec[i].DeliveryDate,
                    ORNumber = (rec[i].Order != null) ? rec[i].Order.ORNumber : null,
                    WhDrNumber = (rec[i].Order != null) ? rec[i].Order.WHDRNumber : null,

                    OrderedBy = (rec[i].Order != null)
                                            ? (
                                                rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery ? rec[i].ClientName
                                                : _context.Stores.Where(y => y.Id == rec[i].Order.StoreId).Select(z => z.Name).FirstOrDefault())
                                                : rec[i].Order.Store.Name
                                            )
                                            : null,
                    OrderedByStore = (rec[i].Order != null)
                                            ? (
                                                rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                ? _context.Stores.Where(y => y.Id == rec[i].Order.StoreId).Select(z => z.Name).FirstOrDefault()
                                                : rec[i].Order.Store.Name
                                            )
                                            : null,

                    OrderedByAddress = (rec[i].Order != null) ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                              ? rec[i].Address1 + " " + rec[i].Address2 + " " + rec[i].Address3
                              : rec[i].Order.Store.Address) : null,
                    OrderedByContact = (rec[i].Order != null) ? (rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery && rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ? rec[i].ContactNumber
                                                                    : rec[i].Order.Store.ContactNumber)
                                                                    : null,
                    OrderedTo = (rec[i].Order != null)
                                            ? (
                                                    (rec[i].Order.Warehouse != null)
                                                    ? rec[i].Order.Warehouse.Name
                                                    :
                                                        (rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                        ? _context.Stores.Where(y => y.Id == rec[i].Order.OrderToStoreId).Select(z => z.Name).FirstOrDefault()
                                                        : null
                                               )
                                            : rec[i].Store.Name,
                    DeliveryType = (rec[i].Order != null) ? rec[i].Order.DeliveryType : DeliveryTypeEnum.Delivery,
                    DeliveryTypeStr = (rec[i].Order != null) ? rec[i].Order.DeliveryTypeStr : EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), DeliveryTypeEnum.Delivery)),
                    ClientPoSP = (rec[i].Order != null) ?
                                        rec[i].Order.DeliveryType == DeliveryTypeEnum.ShowroomPickup && rec[i].Order.OrderType == OrderTypeEnum.ClientOrder ? true : false
                                        : false,
                    rec[i].ApprovedDeliveryDate,
                    DeliveryQty = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                                ? rec[i].ShowroomDeliveries.Sum(p => p.Quantity)
                                                : rec[i].ClientDeliveries.Sum(p => p.Quantity),
                    PODate = (rec[i].Order != null) ? rec[i].Order.PODate : null,
                    PONumber = (rec[i].Order != null) ? rec[i].Order.PONumber : null,

                    RequestStatus = (rec[i].Order != null) ? rec[i].Order.RequestStatus : null,
                    RequestStatusStr = (rec[i].Order != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), rec[i].Order.RequestStatus)) : null,
                    rec[i].ClientName,
                    PreferredTimeStr = (rec[i].PreferredTime != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(PreferredTimeEnum), rec[i].PreferredTime)) : null,
                    rec[i].Address1,
                    rec[i].Address2,
                    rec[i].Address3,
                    rec[i].ContactNumber,
                    rec[i].DriverName,
                    rec[i].PlateNumber,
                    DisplayWHDRNumber,
                    Remarks = (rec[i].Order != null) ? rec[i].Order.Remarks : null,
                    Deliveries = (rec[i].ShowroomDeliveries != null && rec[i].ShowroomDeliveries.Count() > 0)
                                                ? rec[i].ShowroomDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                })
                                                : rec[i].ClientDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                }),
                    DeliveryStatus,
                    DeliveryStatusStr,
                    rec[i].Delivered,
                    DeliveredStr = (rec[i].Delivered != null) ? this.GetDeliveryStatusStr(rec[i].Delivered) : null,
                    IsCustomDelivery = (rec[i].Delivered != null) ? true : false,
                    IsUpdatable = (rec[i].Delivered == null || rec[i].Delivered == DeliveryStatusEnum.Delivered) ? false : (rec[i].Delivered == DeliveryStatusEnum.Waiting) ? true : false,
                    ShowDeliveredButton = (rec[i].Order != null)
                                            ? (((rec[i].Order.OrderType == OrderTypeEnum.ClientOrder || rec[i].Order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder) && (rec[i].Delivered == DeliveryStatusEnum.Waiting || rec[i].Delivered == DeliveryStatusEnum.Delivered))
                                                    && rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClient = (rec[i].Order != null)
                                            ? (rec[i].Order.OrderType == OrderTypeEnum.ClientOrder && rec[i].Order.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                    IsClientPickup = (rec[i].Order != null)
                                            ? (rec[i].Order.OrderType == OrderTypeEnum.ClientOrder && rec[i].Order.DeliveryType == DeliveryTypeEnum.Pickup)
                                                ? true : false
                                            : false,
                    IsInterBranch,
                    TransferHeader,
                    IsTransferShowroomPickup
                };
                //added for ticket #261 client po pickup will not be included in the list.
                if(!obj.IsClientPickup)
                {
                    retList.Add(obj);
                }

       
                
            }

            return retList;


        }

        public IEnumerable<object> GetAllDeliveriesForSales(SearchDeliveries search, IMapper mapper)
        {
            IQueryable<STDelivery> list = _context.STDeliveries
                                            .Include(p => p.ClientDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STSalesId != null);


            var deliveries = new List<STDeliveryDTO>();

            foreach (var del in list)
            {
                var mappedDelivery = mapper.Map<STDeliveryDTO>(del);

                var order = _context.STSales.Where(p => p.Id == del.STSalesId).FirstOrDefault();
                if (order != null)
                {
                    var mappedOrder = mapper.Map<STSalesDTO>(order);
                    mappedDelivery.Sales = mappedOrder;


                    var store = _context.Stores.Where(p => p.Id == del.StoreId).FirstOrDefault();
                    if (store != null)
                    {
                        var mappedStore = mapper.Map<StoreDTO>(store);
                        mappedDelivery.Sales.Store = mappedStore;
                    }

                }

                deliveries.Add(mappedDelivery);
            }

            var query = deliveries.AsEnumerable();

            // Display delivery type only
            query = query.Where(p => p.Sales.DeliveryType != DeliveryTypeEnum.Pickup);


            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                //  Searched by DRNumber
                query = query.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                query = query.Where(y => search.DeliveryDateFrom <= y.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                query = query.Where(y => search.DeliveryDateTo >= y.DeliveryDate);
            }

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.Sales.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.SINumber,
                              x.ORNumber,
                              x.DRNumber,
                              DeliveryRemarks = x.Remarks,
                              TransactionNo = x.Sales.TransactionNo,
                              RequestedDeliveryDate = x.DeliveryDate,
                              SalesType = x.Sales.SalesType,
                              SalesTypeStr = x.Sales.SalesTypeStr,
                              OrderedBy = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.Name).FirstOrDefault(),
                              DeliveryType = x.Sales.DeliveryType,
                              DeliveryTypeStr = x.Sales.DeliveryTypeStr,
                              DeliveryStatus = x.ClientDeliveries.Where(p => p.STDeliveryId == x.Id).Select(p => p.DeliveryStatus).FirstOrDefault(),
                              DeliveryStatusStr = this.GetDeliveryStatusStr(x.ClientDeliveries.Where(p => p.STDeliveryId == x.Id).Select(p => p.DeliveryStatus).FirstOrDefault()),
                              OrderedDate = x.Sales.SalesDate,
                              x.Remarks,
                              AgentName = x.Sales.SalesAgent,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.ApprovedDeliveryDate,
                              x.DriverName,
                              x.PlateNumber,
                              x.Delivered,
                              DeliveredStr = (x.Delivered != null) ? this.GetDeliveryStatusStr(x.Delivered) : null,
                              IsCustomDelivery = (x.Delivered != null) ? true : false,
                              IsUpdatable = (x.Delivered == null) ? false : (x.Delivered == DeliveryStatusEnum.Waiting) ? true : false,
                              ShowDeliveredButton = (x.Sales != null)
                                            ? ((x.Delivered == DeliveryStatusEnum.Waiting || x.Delivered == DeliveryStatusEnum.Delivered) && x.Sales.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                              DeliveryQty = x.ClientDeliveries.Sum(p => p.Quantity),
                              Deliveries = (x.ShowroomDeliveries != null && x.ShowroomDeliveries.Count() > 0)
                                                ? x.ShowroomDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                })
                                                : x.ClientDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                })
                          };

            if (search.DeliveryStatus != null)
            {
                records = records.Where(p => p.IsCustomDelivery
                                ? search.DeliveryStatus.Contains(p.Delivered) : search.DeliveryStatus.Contains(p.DeliveryStatus));

            }


            return records.OrderByDescending(p => p.Id);


        }


        public object GetAllDeliveriesForSalesPaged(SearchDeliveries search, IMapper mapper, AppSettings appSettings)
        {
            IQueryable<STDelivery> query = _context.STDeliveries
                                            .Include(p => p.ClientDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STSalesId != null);


            //var deliveries = new List<STDeliveryDTO>();

            //foreach (var del in list)
            //{
            //    var mappedDelivery = mapper.Map<STDeliveryDTO>(del);

            //    var order = _context.STSales.Where(p => p.Id == del.STSalesId).FirstOrDefault();
            //    if (order != null)
            //    {
            //        var mappedOrder = mapper.Map<STSalesDTO>(order);
            //        mappedDelivery.Sales = mappedOrder;


            //        var store = _context.Stores.Where(p => p.Id == del.StoreId).FirstOrDefault();
            //        if (store != null)
            //        {
            //            var mappedStore = mapper.Map<StoreDTO>(store);
            //            mappedDelivery.Sales.Store = mappedStore;
            //        }

            //    }

            //    deliveries.Add(mappedDelivery);
            //}

            //var query = deliveries.AsEnumerable();

            // Display delivery type only
            query = query.Where(p => p.Sales.DeliveryType != DeliveryTypeEnum.Pickup);


            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                //  Searched by DRNumber
                query = query.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                query = query.Where(y => search.DeliveryDateFrom <= y.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                query = query.Where(y => search.DeliveryDateTo >= y.DeliveryDate);
            }

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.Sales.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.SINumber,
                              x.ORNumber,
                              x.DRNumber,
                              DeliveryRemarks = x.Remarks,
                              TransactionNo = x.Sales.TransactionNo,
                              RequestedDeliveryDate = x.DeliveryDate,
                              SalesType = x.Sales.SalesType,
                              SalesTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), x.Sales.SalesType)),
                              OrderedBy = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.Name).FirstOrDefault(),
                              DeliveryType = x.Sales.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.Sales.DeliveryType)),
                              DeliveryStatus = x.ClientDeliveries.Where(p => p.STDeliveryId == x.Id).Select(p => p.DeliveryStatus).FirstOrDefault(),
                              DeliveryStatusStr = this.GetDeliveryStatusStr(x.ClientDeliveries.Where(p => p.STDeliveryId == x.Id).Select(p => p.DeliveryStatus).FirstOrDefault()),
                              OrderedDate = x.Sales.SalesDate,
                              x.Remarks,
                              AgentName = x.Sales.SalesAgent,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.ApprovedDeliveryDate,
                              x.DriverName,
                              x.PlateNumber,
                              x.Delivered,
                              DeliveredStr = (x.Delivered != null) ? this.GetDeliveryStatusStr(x.Delivered) : null,
                              IsCustomDelivery = (x.Delivered != null) ? true : false,
                              IsUpdatable = (x.Delivered == null) ? false 
                                                                  : (x.Delivered == DeliveryStatusEnum.Waiting && x.ClientDeliveries.Where(p => p.STDeliveryId == x.Id).Select(p => p.DeliveryStatus).FirstOrDefault() == DeliveryStatusEnum.Delivered) ? true : false,
                              ShowDeliveredButton = (x.Sales != null)
                                            ? ((x.Delivered == DeliveryStatusEnum.Waiting || x.Delivered == DeliveryStatusEnum.Delivered) && x.Sales.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? true : false
                                            : false,
                              DeliveryQty = x.ClientDeliveries.Sum(p => p.Quantity),
                              Deliveries = (x.ShowroomDeliveries != null && x.ShowroomDeliveries.Count() > 0)
                                                ? x.ShowroomDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity,
                                                    p.isTonalityAny,
                                                })
                                                : x.ClientDeliveries.Select(p => new
                                                {
                                                    p.Item.SerialNumber,
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity,
                                                    p.isTonalityAny
                                                })
                          };

            records = records.OrderByDescending(p => p.Id);

            if (search.DeliveryStatus != null)
            {
                records = records.Where(p => p.IsCustomDelivery
                                ? search.DeliveryStatus.Contains(p.Delivered) : search.DeliveryStatus.Contains(p.DeliveryStatus));

            }


            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(records.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                records = records.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage);
            }
            else
            {
                response = new GetAllResponse(records.Count());
            }

            response.List.AddRange(records);


            return response;


        }

        //private string GetOrderedBy(STDeliveryDTO model)
        //{
        //    model.STSalesId
        //}

        /// <summary>
        /// Get sales order deliveries records
        /// </summary>
        /// <param name="id">STSales.Id</param>
        /// <param name="storeId">Store.Id</param>
        /// <returns></returns>
        public object GetSalesOrderDeliveriesBySalesId(int? id, int? storeId)
        {
            var record = _context.STSales
                                 .Include(p => p.SoldItems)
                                    .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                                 .Include(p => p.Deliveries)
                                    .ThenInclude(p => p.ClientDeliveries)
                                        .ThenInclude(p => p.Item)
                                            .ThenInclude(p => p.Size)
                                 .Where(p => p.StoreId == storeId
                                             && p.Id == id)
                                 .FirstOrDefault();

            var salesDeliveries = new
            {
                record.Id,
                record.SINumber,
                record.ORNumber,
                record.DRNumber,
                record.TransactionNo,
                record.SalesDate,
                record.DeliveryType,
                record.ClientName,
                record.Address1,
                record.Address2,
                record.Address3,
                record.ContactNumber,
                record.Remarks,
                SoldItems = record.SoldItems.Select(p => new
                {
                    p.STSalesId,
                    p.Id,
                    itemId = p.Item.Id,
                    p.Item.Code,
                    ItemName = p.Item.Name,
                    SizeName = p.Item.Size.Name,
                    p.Item.Tonality,
                    p.Quantity
                }).OrderBy(p => p.Id),

                Deliveries = record.Deliveries.Select(p => new
                {
                    p.Id,
                    p.SINumber,
                    p.ORNumber,
                    p.DRNumber,
                    p.DeliveryDate,
                    p.ApprovedDeliveryDate,
                    p.PreferredTime,
                    p.ClientName,
                    p.Address1,
                    p.Address2,
                    p.Address3,
                    p.ContactNumber,
                    p.Remarks,
                    ReleaseStatus = p.ClientDeliveries.Select(x => x.ReleaseStatus).FirstOrDefault(),
                    ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ClientDeliveries.Select(x => x.ReleaseStatus).FirstOrDefault())),
                    Items = p.ClientDeliveries.Select(q => new
                    {
                        q.Item.Id,
                        q.Item.Code,
                        ItemName = q.Item.Name,
                        SizeName = q.Item.Size.Name,
                        q.Item.Tonality,
                        q.Quantity,
                        q.ReleaseStatus
                    })
                }).OrderBy(p => p.Id),

                RemainingForDelivery = GetRemainingForDelivery(record)
            };



            return salesDeliveries;
        }

        private int GetRemainingForDelivery(STSales record)
        {

            var soldQty = Convert.ToInt32(record.SoldItems.Sum(p => p.Quantity));


            int total = 0;
            foreach (var del in record.Deliveries)
            {
                total += Convert.ToInt32(del.ClientDeliveries.Sum(p => p.Quantity));
            }
            return soldQty - total;
        }

        public void InsertSalesOrderDelivery(ISTStockService stockService, STDelivery delivery)
        {
            var deliveryType = _context.STSales.Where(p => p.Id == delivery.Id).Select(p => p.DeliveryType).FirstOrDefault();

            delivery.STSalesId = delivery.Id;
            delivery.Id = 0;

            if (deliveryType == DeliveryTypeEnum.Pickup)
            {
                delivery.ApprovedDeliveryDate = delivery.DeliveryDate;
            }
            delivery.Delivered = DeliveryStatusEnum.Pending;
            delivery.DateCreated = DateTime.Now;

            foreach (var deliveredItem in delivery.ClientDeliveries)
            {

                //  Check if request deliver quantity is 0
                if (deliveredItem.Quantity == 0)
                {
                    //  Skip record
                    continue;
                }

                deliveredItem.Id = 0;
                deliveredItem.DateCreated = DateTime.Now;

                if (deliveryType == DeliveryTypeEnum.Pickup)
                {
                    deliveredItem.DeliveryStatus = DeliveryStatusEnum.Waiting;
                    deliveredItem.ReleaseStatus = ReleaseStatusEnum.Waiting;
                }
                else
                {
                    deliveredItem.DeliveryStatus = DeliveryStatusEnum.Pending;
                    deliveredItem.ReleaseStatus = ReleaseStatusEnum.Pending;
                }


                _context.STClientDeliveries.Add(deliveredItem);
                _context.SaveChanges();

                if (deliveryType == DeliveryTypeEnum.Pickup)
                {
                    // Set For Release for Sales Order - Pickup
                    var stStock = new STStock
                    {
                        StoreId = delivery.StoreId,
                        STClientDeliveryId = deliveredItem.Id,
                        DeliveryStatus = DeliveryStatusEnum.Waiting,
                        ReleaseStatus = ReleaseStatusEnum.Waiting,
                        ItemId = deliveredItem.ItemId,
                        OnHand = -deliveredItem.Quantity
                    };

                    stockService.InsertSTStock(stStock);
                }


            }

            //  Filter only record with request deliver quantity more than 0
            delivery.ClientDeliveries = delivery.ClientDeliveries.Where(p => p.Quantity > 0).ToList();

            _context.STDeliveries.Add(delivery);
            _context.SaveChanges();
        }

        public STDelivery GetSalesDeliveryForReleasingByIdAndStoreId(int id, int? storeId)
        {
            var record = _context.STDeliveries.Include(p => p.ClientDeliveries).Where(p => p.StoreId == storeId
                                                          && p.Id == id
                                                          && p.ApprovedDeliveryDate.HasValue)
                                              .FirstOrDefault();

            if (record != null)
            {
                var totalNotDelivered = record.ClientDeliveries.Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                                           && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                                               .Count();

                if (totalNotDelivered == 0)
                {
                    return null;
                }
            }

            return record;

        }

        public void UpdateSalesDeliveryForReleasing(ISTDeliveryService deliveryService, ISTStockService stockService, STDelivery param)
        {
            param.DateUpdated = DateTime.Now;

            foreach (var delivery in param.ClientDeliveries)
            {
                delivery.DateUpdated = DateTime.Now;
                delivery.DeliveryStatus = DeliveryStatusEnum.Delivered;
                delivery.ReleaseStatus = ReleaseStatusEnum.Released;

                var stStock = new STStock
                {
                    StoreId = param.StoreId,
                    STClientDeliveryId = delivery.Id,
                    DeliveryStatus = DeliveryStatusEnum.Delivered,
                    ReleaseStatus = ReleaseStatusEnum.Released,
                    ItemId = delivery.ItemId,
                    OnHand = -delivery.Quantity
                };

                stockService.InsertSTStock(stStock);

            }

            _context.STDeliveries.Update(param);
            _context.SaveChanges();


            var sales = _context.STSales.Include(p => p.SoldItems).Where(p => p.Id == param.STSalesId && p.StoreId == param.StoreId).FirstOrDefault();
            if (sales != null)
            {
                foreach (var detail in sales.SoldItems)
                {
                    var totalDeliveredQty = Convert.ToInt32(
                                                _context.STClientDeliveries
                                                    .Where(p => p.STSalesDetailId == detail.Id
                                                               && p.ItemId == detail.ItemId
                                                               && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                               && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Sum(p => p.Quantity)
                                            );

                    if (detail.Quantity == totalDeliveredQty)
                    {
                        sales.ReleaseDate = DateTime.Now;

                        _context.STSales.Update(sales);
                        _context.SaveChanges();
                    }
                }
            }

        }

        private string GetDeliveryStatusStr(DeliveryStatusEnum? deliveryStatus)
        {
            return (deliveryStatus.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), deliveryStatus)) : null;
        }
    }
}
