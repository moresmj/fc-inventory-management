using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FC.Api.Services.Warehouses.PhysicalCount
{
    public class UploadPhysicalCountService : IUploadPhysicalCountService
    {

        private DataContext _context;
        private WHStockService _warehouseStockService;
        public UploadPhysicalCountService(DataContext context,IWHStockService whStockService)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public void SavePhysicalCount(WHImport param)
        {
            var totalRecordCount = Convert.ToInt32(this._context.WHImports
                                                                .Count() + 1
                                                   ).ToString();

            param.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateUploaded = DateTime.Now;
            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;
            
            foreach (var dtl in param.Details)
            {
                dtl.DateCreated = DateTime.Now;
            }

            _context.WHImports.Add(param);
            _context.SaveChanges();

        }


        public string SavePhysicalCount2(WHImport param)
        {
            var totalRecordCount = Convert.ToInt32(this._context.WHImports
                                                                .Count() + 1
                                                   ).ToString();

            param.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateUploaded = DateTime.Now;
            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;
            param.PhysicalCountType = PhysicalCountTypeEnum.Upload;
            _warehouseStockService = new WHStockService(_context);
            _warehouseStockService.whStock = _context.WHStocks.AsNoTracking();
            foreach (var dtl in param.Details)
            {
                dtl.DateCreated = DateTime.Now;
                var OnHand = GetDiscrepancy(dtl, param.WarehouseId);
                //dtl.SystemCount = OnHand;
                var systemCount = _warehouseStockService.GetItemAvailableQuantity(dtl.ItemId, param.WarehouseId, false);
                var broken = _warehouseStockService.GetItemBrokenQuantity(dtl.ItemId, param.WarehouseId);
                dtl.SystemCount = systemCount + broken;
            }

            _context.WHImports.Add(param);
            _context.SaveChanges();

            return param.TransactionNo;
        }

        public string SavePhysicalCount3(WHImport param)
        {
            var totalRecordCount = Convert.ToInt32(this._context.WHImports
                                                                .Count() + 1
                                                   ).ToString();

            param.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateUploaded = DateTime.Now;
            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;
            param.PhysicalCountType = PhysicalCountTypeEnum.Breakage;
            _warehouseStockService = new WHStockService(_context);
            _warehouseStockService.whStock = _context.WHStocks.AsNoTracking();
            foreach (var dtl in param.Details)
            {
                dtl.DateCreated = DateTime.Now;
                var OnHand = GetDiscrepancy(dtl, param.WarehouseId);
                //dtl.SystemCount = OnHand;
                var systemCount = _warehouseStockService.GetItemAvailableQuantity(dtl.ItemId, param.WarehouseId, false);
                var broken = _warehouseStockService.GetItemBrokenQuantity(dtl.ItemId, param.WarehouseId);
                dtl.SystemCount = systemCount + broken;
            }

            _context.WHImports.Add(param);
            _context.SaveChanges();

            return param.TransactionNo;

        }


        public string SaveReserveAdjustment(AdjustReservedItemQuantity param, IWHStockService whStockService)
        {
            var totalRecordCount = Convert.ToInt32(this._context.WHImports
                                                                .Count() + 1
                                                   ).ToString();

            var import = new WHImport();

            import.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            import.DateUploaded = DateTime.Now;
            import.DateCreated = DateTime.Now;
            import.RequestStatus = RequestStatusEnum.Approved;
            import.PhysicalCountType = PhysicalCountTypeEnum.AdjustReserved;
            import.WarehouseId = param.WarehouseId;
            _warehouseStockService = new WHStockService(_context);
            _warehouseStockService.whStock = _context.WHStocks.AsNoTracking();
            var detailList = new List<WHImportDetail>();

            foreach (var dtl in param.Details)
            {
                var importDetails = new WHImportDetail();
                importDetails.DateCreated = DateTime.Now;
                importDetails.ReserveAdjustment = dtl.Adjustment;
                importDetails.ItemId = dtl.ItemId;
                importDetails.Remarks = dtl.Remarks;

                detailList.Add(importDetails);
       
                
                
                //var whstock = new WHStock
                //{
                //    WarehouseId = dtl.WarehouseId,
                //    ItemId = dtl.ItemId,
                //    OnHand = dtl.Adjustment,
                //    Reserved = -dtl.Adjustment,
                //    TransactionType = TransactionTypeEnum.PhysicalCount,
                //    DeliveryStatus = DeliveryStatusEnum.Delivered,
                //};

                //whStockService.InsertStock(whstock);
            }

            import.Details = detailList;

            _context.WHImports.Add(import);
            _context.SaveChanges();

            this.InserToWhStock(import.Id, whStockService);

            return import.TransactionNo;

        }


        private void InserToWhStock(int? importId, IWHStockService wHStockService)
        {
            var obj = _context.WHImports
                        .Include(p => p.Details)
                        .Where(p => p.Id == importId
                                    && p.RequestStatus == RequestStatusEnum.Approved)
                        .FirstOrDefault();

            foreach (var dtl in obj.Details)
            {
                var whstock = new WHStock
                {
                    WarehouseId = obj.WarehouseId,
                    ItemId = dtl.ItemId,
                    OnHand = dtl.ReserveAdjustment,
                    Reserved = -dtl.ReserveAdjustment,
                    TransactionType = TransactionTypeEnum.PhysicalCount,
                    DeliveryStatus = DeliveryStatusEnum.Delivered,
                    WHImportDetailId = dtl.Id
                };

                wHStockService.InsertStock(whstock);

            }


        }


        public IEnumerable<object> GetAll()
        {
            IQueryable<WHImport> query = _context.WHImports
                                                 .Include(p => p.Warehouse)
                                                 .Include(p => p.Details)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                 .Where(p => p.RequestStatus == RequestStatusEnum.Pending);

            var records = from x in query
                          select new
                          {
                              x.Id,
                              Warehouse = x.Warehouse.Code,
                              x.DateUploaded
                          };

            return records;

        }

        public ApproveWarehousePhysicalCountDTO GetByTransNo(string transNo)
        {
            var record = _context.WHImports
                               .Include(p => p.Warehouse)
                               .Include(p => p.Details)
                               .ThenInclude(p => p.Item)
                                  .ThenInclude(p => p.Size)
                               .Where(p => p.RequestStatus == RequestStatusEnum.Pending
                                           && p.TransactionNo == transNo)
                               .FirstOrDefault();




            var details = record.Details.Select(p => new
            {
                p.Id,
                p.WHImportId,
                p.ItemId,
                p.AllowUpdate,
            });


            ICollection<ApproveWarehousePhysicalCountItems> physicalCountItems = new Collection<ApproveWarehousePhysicalCountItems>(); 
            foreach (var detail in details)
            {
                physicalCountItems.Add(new ApproveWarehousePhysicalCountItems
                {
                    Id = detail.Id,
                    WHImportId = detail.WHImportId,
                    ItemId = detail.ItemId,
                    AllowUpdate = true,

                });


            }

            var approvedPcount = new ApproveWarehousePhysicalCountDTO
            {
                Id = record.Id,
                Details = physicalCountItems
            };

            return approvedPcount;
        }

        public object GetById(int id)
        {
            var record = _context.WHImports
                                 .Include(p => p.Warehouse)
                                 .Include(p => p.Details)
                                 .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                                 .Where(p => p.RequestStatus == RequestStatusEnum.Pending
                                             && p.Id == id)
                                 .FirstOrDefault();

            if(record != null)
            {

                return
                        new
                        {
                            record.Id,
                            Warehouse = record.Warehouse.Code,
                            record.DateUploaded,
                            Details = record.Details.Select(p => new
                            {
                                p.Id,
                                p.ItemId,
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                p.Item.Tonality,
                                SizeName = p.Item.Size.Name,
                                p.SystemCount,
                                p.PhysicalCount,
                                Discrepancy = GetDiscrepancy(p,record.WarehouseId)
                            }).Where(p=>p.Discrepancy !=0)
                        };
            }

            return null;
        }

        public int GetById2(int id)
        {
            var count = 0;
            var record = _context.WHImports
                                 .Include(p => p.Warehouse)
                                 .Include(p => p.Details)
                                 .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                                 .Where(p => p.RequestStatus == RequestStatusEnum.Pending
                                             && p.Id == id)
                                 .FirstOrDefault();

            if (record != null)
            {
                count = record.Details.Select(p => new
                {
                    Discrepancy = GetDiscrepancy(p,record.WarehouseId)
                }).Where(p => p.Discrepancy != 0).Count();


            }

            return count;
        }

        private int GetDiscrepancy(WHImportDetail p,int? warehouseId)
        {
     

            var systemCount = _warehouseStockService.GetItemAvailableQuantity(p.ItemId, warehouseId, false);
            var broken = _warehouseStockService.GetItemBrokenQuantity(p.ItemId, warehouseId);
            var breakage = _warehouseStockService.GetItemBreakageQuantity(p.ItemId, warehouseId);

            if(breakage < 0)
            {
                breakage = 0;
            }
            var onHand = systemCount + broken + breakage;

            return Convert.ToInt32(onHand - p.PhysicalCount) < 0
                                                        ? Math.Abs(Convert.ToInt32(onHand - p.PhysicalCount))
                                                        : -Convert.ToInt32(onHand - p.PhysicalCount);
        }




        private int GetItemBrokenQuantity(int? itemId, int? warehouseId, int? csvBroken)
        {
            var total = Convert.ToInt32(
                                            _context.WHStocks
                                                .Where(p =>
                                                            p.WarehouseId == warehouseId
                                                            && p.ItemId == itemId
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                            && p.Broken == true
                                                        )
                                                .Sum(y => y.OnHand)
                                        );


            if (total == 0)
            {
                return total = (int)csvBroken;
            }
            else
            {
                return total = (int)csvBroken - total;
            }
                
        }

        public void Approve(ApproveWarehousePhysicalCountDTO dto, IWHStockService whStockService)
        {
            _warehouseStockService = new WHStockService(_context);
            _warehouseStockService.whStock = _context.WHStocks.AsNoTracking();
            _warehouseStockService.stOrder = _context.STOrders.AsNoTracking().Include(p => p.OrderedItems);
            var obj = _context.WHImports
                              .Where(p => p.Id == dto.Id
                                          && p.RequestStatus == RequestStatusEnum.Pending)
                              .FirstOrDefault();
            if(obj != null)
            {
                obj.RequestStatus = RequestStatusEnum.Approved;
                obj.DateUpdated = DateTime.Now;
                obj.DateApproved = DateTime.Now;

                foreach(var dtl in dto.Details)
                {
                    var objImportDtl = obj.Details
                                          .Where(p => p.Id == dtl.Id
                                                      && p.ItemId == dtl.ItemId
                                                      && p.WHImportId == dtl.WHImportId)
                                          .FirstOrDefault();
                    if(objImportDtl != null)
                    {
                        objImportDtl.AllowUpdate = dtl.AllowUpdate;
                        objImportDtl.DateUpdated = DateTime.Now;

                        if(dtl.AllowUpdate == true)
                        {
                            if (obj.PhysicalCountType != PhysicalCountTypeEnum.Breakage)
                            {
                                var whStock = new WHStock
                                {
                                    WarehouseId = obj.WarehouseId,
                                    ItemId = dtl.ItemId,
                                    WHImportDetailId = objImportDtl.Id,
                                    OnHand = GetDiscrepancy(objImportDtl, obj.WarehouseId),
                                    TransactionType = TransactionTypeEnum.PhysicalCount,
                                    DeliveryStatus = DeliveryStatusEnum.Delivered
                                };



                                whStockService.InsertStock(whStock);
                            }

                            if (objImportDtl.Broken.HasValue)
                            {
                                var whbrStock = new WHStock
                                {
                                    WarehouseId = obj.WarehouseId,
                                    ItemId = dtl.ItemId,
                                    OnHand = objImportDtl.Broken * -1,//GetItemBrokenQuantity(dtl.ItemId,obj.WarehouseId, objImportDtl.Broken),
                                    WHImportDetailId = objImportDtl.Id,
                                    TransactionType = TransactionTypeEnum.PhysicalCount,
                                    DeliveryStatus = DeliveryStatusEnum.Delivered,
                                    Broken = true
                                };
                             
                                if(whbrStock.OnHand != 0)
                                {
                                    whStockService.InsertStock(whbrStock);
                                }
                                    
                          
                                
                            }

                            //UpdateWarehouseInventory(dtl.ItemId.Value, obj.WarehouseId.Value);
                        }
                    }
                }

                _context.WHImports.Update(obj);
                _context.SaveChanges();
            }

        }


        private void UpdateWarehouseInventory(int ItemId, int warehouseId)
        {
            var rec = _context.WHStockSummary.Where(x => x.ItemId == ItemId).FirstOrDefault();
            if (rec == null)
            {
                return;
            }


            var approvedOrderedItem = _warehouseStockService.GetApprovedOrderedItem(ItemId, warehouseId);

            #region For Release
            var totalItemForReleasing = _warehouseStockService.GetItemForReleasing(ItemId, warehouseId, approvedOrderedItem);

            rec.ForRelease = totalItemForReleasing;

            #endregion

            #region On-hand

            var totalItemAvailable = _warehouseStockService.GetItemAvailableQuantity(rec.ItemId, warehouseId, false);

            var totalItemReceivedBroken = _warehouseStockService.GetItemBrokenQuantity(rec.ItemId, warehouseId);

            var totalItemBreakage = _warehouseStockService.GetItemBreakageQuantity(rec.ItemId, warehouseId);

            totalItemAvailable = totalItemAvailable + totalItemBreakage;

            #endregion

            rec.OnHand = totalItemAvailable + totalItemReceivedBroken;
            rec.Broken = totalItemReceivedBroken + (totalItemBreakage * -1);

            rec.Available = totalItemAvailable - totalItemForReleasing;

            _context.WHStockSummary.Update(rec);
            _context.SaveChanges();
        }
    }
}
