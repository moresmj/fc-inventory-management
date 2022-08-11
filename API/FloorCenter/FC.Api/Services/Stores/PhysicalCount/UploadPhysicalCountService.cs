using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FC.Api.Services.Stores.PhysicalCount
{
    public class UploadPhysicalCountService : IUploadPhysicalCountService
    {

        private DataContext _context;
        private STStockService _storeStockService;
        public UploadPhysicalCountService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public void SavePhysicalCount(STImport param)
        {
            var totalRecordCount = Convert.ToInt32(this._context.STImports
                                                    .Count() + 1
                                       ).ToString();

            param.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateUploaded = DateTime.Now;
            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;



            var stService = new STStockService(_context);
            stService.stStock = _context.STStocks;
            foreach (var dtl in param.Details)
            {
                dtl.DateCreated = DateTime.Now;
                var totalItemAvailable = stService.GetItemAvailableQuantity(dtl.ItemId, param.StoreId, false);
                var totalItemBroken = stService.GetItemBrokenQuantity(dtl.ItemId, param.StoreId);
                dtl.SystemCount = totalItemAvailable + totalItemBroken;
            }

            _context.STImports.Add(param);
            _context.SaveChanges();

        }

        public string SavePhysicalCount2(STImport param)
        {

            var totalRecordCount = Convert.ToInt32(this._context.STImports
                                                    .Count() + 1
                                       ).ToString();

            param.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateUploaded = DateTime.Now;
            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;



            var stService = new STStockService(_context);
            stService.stStock = _context.STStocks.AsNoTracking();
            foreach (var dtl in param.Details)
            {
                dtl.DateCreated = DateTime.Now;
                var totalItemAvailable = stService.GetItemAvailableQuantity(dtl.ItemId, param.StoreId, false);
                var totalItemBroken = stService.GetItemBrokenQuantity(dtl.ItemId, param.StoreId);
                dtl.SystemCount = totalItemAvailable + totalItemBroken;
            }

            _context.STImports.Add(param);
            _context.SaveChanges();
    
 
            return param.TransactionNo;
        }

        public string SaveBreakage(STImport param)
        {
            var totalRecordCount = Convert.ToInt32(this._context.STImports
                                                    .Count() + 1
                                       ).ToString();

            param.TransactionNo = string.Format("PC{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateUploaded = DateTime.Now;
            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;

            var stService = new STStockService(_context);
            stService.stStock = _context.STStocks;
            foreach (var dtl in param.Details)
            {
                dtl.DateCreated = DateTime.Now;
                var totalItemAvailable = stService.GetItemAvailableQuantity(dtl.ItemId, param.StoreId, false);
                var totalItemBroken = stService.GetItemBrokenQuantity(dtl.ItemId, param.StoreId);
                dtl.SystemCount = totalItemAvailable + totalItemBroken;
            }

            _context.STImports.Add(param);
            _context.SaveChanges();


            return param.TransactionNo;
        }

        public IEnumerable<object> GetAll(ApprovePhysicalCountSearchDTO search)
        {
            IQueryable<STImport> query = _context.STImports
                                                 .Include(p => p.Store).OrderByDescending(p => p.Id);
                                                    /*.Where(p => p.RequestStatus == RequestStatusEnum.Pending).*/

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }
         

            var records = from x in query
                          select new
                          {
                              x.Id,
                              Store = x.Store.Name,
                              x.DateUploaded,
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum),x.RequestStatus))
                          };

            return records;

        }

        public object GetById(int id)
        {
            var record = _context.STImports
                                 .Include(p => p.Store)
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
                            Store = record.Store.Name,
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
                                p.Remarks,
                                Discrepancy = GetDiscrepancy(p)
                            })/*.Where(p => p.Discrepancy != 0)*/
                        };
            }

            return null;
        }

        public int GetById2(int id)
        {
            var count = 0;
            var record = _context.STImports
                                 .Include(p => p.Store)
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
                    Discrepancy = GetDiscrepancy(p)
                }).Where(p => p.Discrepancy != 0).Count();


            }

            return count;
        }

        private static int GetDiscrepancy(STImportDetail p)
        {
            return Convert.ToInt32(p.SystemCount - p.PhysicalCount) < 0
                                                        ? Math.Abs(Convert.ToInt32(p.SystemCount - p.PhysicalCount))
                                                        : -Convert.ToInt32(p.SystemCount - p.PhysicalCount);
        }

        public void Approve(ApproveStorePhysicalCountDTO dto, ISTStockService stStockService)
        {
            var obj = _context.STImports
                              .Where(p => p.Id == dto.Id
                                          && p.RequestStatus == RequestStatusEnum.Pending)
                              .FirstOrDefault();
            if(obj != null)
            {
                var declinedCount = dto.Details.Where(p => p.AllowUpdate == false).Count();
                var dtlCount = dto.Details.Count();
               //check if the request is cancelled or approved
                    if (declinedCount == dtlCount)
                    {
                        obj.RequestStatus = RequestStatusEnum.Cancelled;
                    }
                    else
                    {
                        obj.RequestStatus = RequestStatusEnum.Approved;
                    }
                
                obj.DateUpdated = DateTime.Now;
                obj.DateApproved = DateTime.Now;

                foreach(var dtl in dto.Details)
                {
                    var objImportDtl = obj.Details
                                          .Where(p => p.Id == dtl.Id
                                                      && p.ItemId == dtl.ItemId
                                                      && p.STImportId == dtl.STImportId)
                                          .FirstOrDefault();
                    if(objImportDtl != null)
                    {
                        objImportDtl.AllowUpdate = dtl.AllowUpdate;
                        objImportDtl.DateUpdated = DateTime.Now;

                        if(dtl.AllowUpdate == true)
                        {
                            var stStock = new STStock
                            {
                                StoreId = obj.StoreId,
                                ItemId = dtl.ItemId,
                                STImportDetailId = objImportDtl.Id,
                                OnHand = GetDiscrepancy(objImportDtl),
                                DeliveryStatus = DeliveryStatusEnum.Delivered
                            };

                            stStockService.InsertSTStock(stStock);
                            //UpdateStoreInventory(dtl.ItemId.Value, obj.StoreId.Value, stStockService);
                        }
                    }
                }

              

                _context.STImports.Update(obj);
                _context.SaveChanges();
            }

        }


        public void ApproveBreakage(ApproveStorePhysicalCountDTO dto, ISTStockService stStockService)
        {
            var obj = _context.STImports
                              .Where(p => p.Id == dto.Id
                                          && p.RequestStatus == RequestStatusEnum.Pending && p.PhysicalCountType == PhysicalCountTypeEnum.Breakage)
                              .FirstOrDefault();
            if (obj != null)
            {
                obj.RequestStatus = RequestStatusEnum.Approved;
                obj.DateUpdated = DateTime.Now;
                obj.DateApproved = DateTime.Now;
                foreach (var dtl in dto.Details)
                {
                    var objImportDtl = obj.Details
                                          .Where(p => p.Id == dtl.Id
                                                      && p.ItemId == dtl.ItemId
                                                      && p.STImportId == dtl.STImportId)
                                          .FirstOrDefault();
                    if (objImportDtl != null)
                    {
                        objImportDtl.AllowUpdate = true;
                        objImportDtl.DateUpdated = DateTime.Now;

                        if (objImportDtl.Broken.HasValue)
                        {
                            var stStock = new STStock
                            {
                                StoreId = obj.StoreId,
                                ItemId = dtl.ItemId,
                                STImportDetailId = objImportDtl.Id,
                                OnHand = objImportDtl.Broken,
                                DeliveryStatus = DeliveryStatusEnum.Delivered,
                                Broken = true
                            };

                            stStockService.InsertSTStock(stStock);

                            //UpdateStoreInventory(dtl.ItemId.Value, obj.StoreId.Value, stStockService);
                        }
                    }
                }

                _context.STImports.Update(obj);
                _context.SaveChanges();
            }

        }


        private void UpdateStoreInventory(int ItemId, int storeId, ISTStockService stStockService)
        {
            var rec = _context.STStockSummary.Where(x => x.ItemId == ItemId && x.StoreId == storeId).FirstOrDefault();
            if (rec == null)
            {
                return;
            }


            //var totalItemForReleasing = stStockService.GetItemForReleasing(rec.ItemId, storeId);

            //rec.ForRelease = totalItemForReleasing;

            var totalItemAvailable = stStockService.GetItemAvailableQuantity(rec.ItemId, storeId, false);

            //var totalItemBroken = stStockService.GetItemBrokenQuantity(rec.ItemId, storeId);

            //rec.Broken = totalItemBroken;

            rec.OnHand = totalItemAvailable + rec.Broken;

            rec.Available = totalItemAvailable - rec.ForRelease;

            _context.STStockSummary.Update(rec);
            _context.SaveChanges();
        }

        public ApproveStorePhysicalCountDTO GetByTransNo(string transNo)
        {
            var record = _context.STImports
                               .Include(p => p.Store)
                               .Include(p => p.Details)
                               .ThenInclude(p => p.Item)
                                  .ThenInclude(p => p.Size)
                               .Where(p => p.RequestStatus == RequestStatusEnum.Pending
                                           && p.TransactionNo == transNo)
                               .FirstOrDefault();




            var details = record.Details.Select(p => new
            {
                p.Id,
                p.STImportId,
                p.ItemId,
                p.AllowUpdate,
            });


            ICollection<ApproveStorePhysicalCountItems> physicalCountItems = new Collection<ApproveStorePhysicalCountItems>();
            foreach (var detail in details)
            {
                physicalCountItems.Add(new ApproveStorePhysicalCountItems
                {
                    Id = detail.Id,
                    STImportId = detail.STImportId,
                    ItemId = detail.ItemId,
                    AllowUpdate = true,

                });


            }

            var approvedPcount = new ApproveStorePhysicalCountDTO
            {
                Id = record.Id,
                Details = physicalCountItems
            };

            return approvedPcount;
        }

    }
}
