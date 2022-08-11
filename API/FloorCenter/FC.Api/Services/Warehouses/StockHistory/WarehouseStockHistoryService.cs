using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.StockHistory;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Warehouses.StockHistory
{
    public class WarehouseStockHistoryService : IWarehouseStockHistoryService
    {

        private DataContext _context;

        public WarehouseStockHistoryService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<StockHistoryDTO> GetMainSelectedWarehouseItemHistory(InventorySearchDTO dto)
        {
            return this.GetByItemIdAndWarehouseId((int)dto.Id, dto.WarehouseId);
        }

        public IEnumerable<StockHistoryDTO> GetByItemIdAndWarehouseId(int id, int? warehouseId)
        {
            IQueryable<WHStock> stocks = _context.WHStocks
                                                 .Include(p => p.STOrderDetail)
                                                 .Include(p => p.WHReceiveDetail)
                                                 .Include(p => p.WHDeliveryDetail)
                                                 .Include(p => p.STShowroomDelivery)
                                                 .Include(p => p.STClientDelivery)
                                                 .Include(p => p.WHImportDetail)
                                                 .Include(p => p.WHAllocateAdvanceOrderDetail)
                                                 .Where(p => p.WarehouseId == warehouseId
                                                             && p.ItemId == id);

            var info = _context.Warehouses
                               .Where(p => p.Id == warehouseId)
                               .FirstOrDefault();

            var records = new List<StockHistoryDTO>();

            foreach(var stock in stocks)
            {
                var history = new StockHistoryDTO
                {
                    Stock = stock.OnHand,
                    Origin = info.Code,
                    TransactionDate = (stock.DateUpdated != null) ? stock.DateUpdated : stock.DateCreated,
                    Broken = stock.Broken,
                    Reserved = stock.Reserved,
                    DeliveryStatus = stock.DeliveryStatus
                };  

                if(stock.STOrderDetailId.HasValue)
                {
                    Get_Info_From_STOrder(stock, history);
                }
                else if(stock.WHReceiveDetailId.HasValue)
                {
                    history.Origin = "Receive From Order";
                    history.Destination = info.Code;
                    Get_Info_From_WHReceive(stock, history);
                }
                else if(stock.WHDeliveryDetailId.HasValue)
                {
                    history.Destination = info.Code;
                    Get_Info_From_WHDelivery(stock, history);
                }
                else if(stock.STShowroomDeliveryId.HasValue)
                {
                    //added for ticket 328 must only display on stock delivery once released on warehouse
                    if(stock.STShowroomDelivery.ReleaseStatus != ReleaseStatusEnum.Released)
                    {
                        continue;
                    }
                    Get_Info_From_STShowroomDelivery(stock, history);
                }
                else if(stock.STClientDeliveryId.HasValue)
                {
                    if(stock.STClientDelivery.ReleaseStatus != ReleaseStatusEnum.Released)
                    {
                        continue;
                    }
                    Get_Info_From_STClientDelivery(stock, history);
                }
                else if(stock.WHImportDetailId.HasValue)
                {
                    Get_Info_From_WHImport(stock, history);
                }
                //Added for advance order 
                else if(stock.WHAllocateAdvanceOrderDetailId.HasValue)
                {
                    Get_Info_From_WHAllocateAdvanceOrder(stock, history);
                }


                records.Add(history);
            }

            records = records.OrderByDescending(p => p.TransactionDate).ToList();

            return records;
        }

        private void Get_Info_From_STOrder(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.STOrders
                              .Include(p => p.Store)
                              .Where(p => p.Id == stock.STOrderDetail.STOrderId)
                              .FirstOrDefault();

            if (obj != null)
            {
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), obj.OrderType));
                history.PONumber = obj.PONumber;
                history.PODate = obj.PODate;
                history.ReleaseDate = obj.ReleaseDate;

                if(obj.Store != null)
                {
                    history.Destination = obj.Store.Name;
                }
            }
        }

        private void Get_Info_From_WHReceive(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.WHReceives
                              .Where(p => p.Id == stock.WHReceiveDetail.WHReceiveId)
                              .Include(p => p.ReceivedItems)
                              .FirstOrDefault();


            if (obj != null)
            {
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum),TransactionTypeEnum.PO));
                history.PONumber = obj.PONumber;
                history.DRNumber = obj.DRNumber;
                history.PODate = obj.PODate;
                history.ReceivedDate = obj.ReceivedDate;
            }
        }

        private void Get_Info_From_WHDelivery(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.WHDeliveries
                              .Include(p => p.Store)
                              .Where(p => p.Id == stock.WHDeliveryDetail.WHDeliveryId)
                              .FirstOrDefault();

            if (obj != null)
            {
                history.DRNumber = obj.DRNumber;
                history.ReleaseDate = obj.ReleaseDate;

                if (obj.Store != null)
                {
                    history.Origin = obj.Store.Name;
                }

                var objSTReturn = _context.STReturns
                                          .Where(p => p.Id == obj.STReturnId)
                                          .FirstOrDefault();
                if(objSTReturn != null)
                {
                    history.TransactionNo = objSTReturn.TransactionNo;
                    history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), objSTReturn.ReturnType));
                }

            }
        }

        private void Get_Info_From_STShowroomDelivery(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.STDeliveries
                              .Include(p => p.Store)
                              .Where(p => p.Id == stock.STShowroomDelivery.STDeliveryId)
                              .FirstOrDefault();

            if (obj != null)
            {
                history.DRNumber = obj.DRNumber;
                history.DeliveryDate = obj.ApprovedDeliveryDate;
                history.ReleaseDate = obj.ReleaseDate;
                history.ORNumber = obj.ORNumber;

                if (obj.Store != null)
                {
                    history.Destination = obj.Store.Name;
                }

                var order = _context.STOrders
                                    .Where(p => p.Id == obj.STOrderId)
                                    .FirstOrDefault();

                if(order != null)
                {
                    history.TransactionNo = order.TransactionNo;
                    history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), order.OrderType));
                    history.PONumber = order.PONumber;
                    history.PODate = order.PODate;
                    if(stock.OnHand < 0)
                    {
                        history.DRNumber = order.WHDRNumber;
                    }
                }
            }
        }

        private void Get_Info_From_STClientDelivery(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.STDeliveries
                              .Include(p => p.Store)
                              .Where(p => p.Id == stock.STClientDelivery.STDeliveryId)
                              .FirstOrDefault();

            if (obj != null)
            {
                history.DRNumber = obj.DRNumber;
                history.DeliveryDate = obj.ApprovedDeliveryDate;
                history.ReleaseDate = obj.ReleaseDate;
                history.ORNumber = obj.ORNumber;

                if (obj.Store != null)
                {
                    history.Destination = obj.Store.Name;
                }

                var order = _context.STOrders
                                    .Where(p => p.Id == obj.STOrderId)
                                    .FirstOrDefault();

                if (order != null)
                {
                    history.TransactionNo = order.TransactionNo;
                    history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), OrderTypeEnum.ClientOrder));
                    history.PONumber = order.PONumber;
                    history.PODate = order.PODate;

                    if(history.Stock < 1)
                    {
                        history.DRNumber = order.WHDRNumber;
                    }
                }
            }
        }

        private void Get_Info_From_WHImport(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.WHImports
                              .Where(p => p.Id == stock.WHImportDetail.WHImportId)
                              .FirstOrDefault();
           
            if (obj != null)
            {
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.PhysicalCount));
                history.Remarks = stock.WHImportDetail.Remarks;
            }

        }

        private void Get_Info_From_WHAllocateAdvanceOrder(WHStock stock, StockHistoryDTO history)
        {
            var obj = _context.WHAllocateAdvanceOrder
                              .Where(p => p.Id == stock.WHAllocateAdvanceOrderDetail.WHAllocateAdvanceOrderId)
                              .FirstOrDefault();
            if (obj != null)
            {
                history.TransactionNo = obj.AllocationNumber;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.AdvanceOrder));
            }

        }

    }
}
