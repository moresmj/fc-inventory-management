using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses.ModifyTonality;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Warehouses
{
    public class ModifyTonalityService : IModifyTonalityService
    {

        private DataContext _context;


        public ModifyTonalityService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }


        public object GetAllForApprovalRequests_ChangeItemTonality(SearchModifyTonality search, AppSettings appSettings)
        {

            IQueryable<WHModifyItemTonality> query = _context.WHModifyItemTonality
                                                               .Include(p => p.ModifyItemTonalityDetails)
                                                                    .ThenInclude(p => p.Item)
                                                                        .ThenInclude(p => p.Size)
                                                                .Include(p => p.Warehouse)
                                                                    .OrderByDescending(p => p.Id);


            if(search.WarehouseId.HasValue)
            {
                query = query.Where(p => p.WarehouseId == search.WarehouseId);
            }

            if(search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }

            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {
                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
            }


            GetAllResponse response = null;

            if(search.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);

                if(search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();

                }

                query = query.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage);
            }
            else
            {
                response = new GetAllResponse(query.Count());
            }
            var stordId = query.Select(p => p.STOrderId);
            var orders = _context.STOrders.Where(p => stordId.Contains(p.Id));

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                              x.DateApproved,
                              x.DateCreated,
                              x.DateUpdated,
                              x.STOrderId,
                              x.WarehouseId,
                              x.Warehouse,
                              TransactionTobeModified = orders.Where(p => p.Id == x.STOrderId).Select(p => p.TransactionNo).FirstOrDefault(),
                              PONumber = orders.Where(p => p.Id == x.STOrderId).Select(p => p.PONumber).FirstOrDefault(),
                              ItemsForModification = x.ModifyItemTonalityDetails.Select(p => new
                              {
                                  p.Item,
                                  OldTonality = _context.Items.Where(t => t.Id == p.OldItemId).Select(t => t.Tonality).FirstOrDefault(),
                                  p.Remarks,
                                  p.Id,
                                  p.WHModifyItemTonalityId,
                                  p.StClientDeliveryId,
                                  p.StShowroomDeliveryId,
                              }),
                              x.ModifyItemTonalityDetails

                          };

            response.List.AddRange(records);

            return response;
        }

        

        public void ApproveModifyTonality(ApproveModifyTonalityDTO param)
        {
            var record = _context.WHModifyItemTonality
                            .Include(p => p.ModifyItemTonalityDetails)
                                .ThenInclude(p => p.Item)
                                    .Include(p => p.Warehouse)
                                        .Where(p => p.Id == param.Id)
                                            .SingleOrDefault();


            this.UpdateModifyTonality(record);

            if (param.RequestStatus == RequestStatusEnum.Approved)
            {



                var order = _context.STOrders
                                        .Include(p => p.OrderedItems)
                                        //.ThenInclude(p => p.Item)
                                        .AsNoTracking()
                                         .Include(p => p.Deliveries)
                                         .AsNoTracking()
                                            .Where(p => p.Id == record.STOrderId).SingleOrDefault();

                var whStocks = _context.WHStocks;
                using (var transaction = _context.Database.BeginTransaction())
                {

                    foreach (var item in record.ModifyItemTonalityDetails)
                    {

                        var rec = _context.STOrderDetails.AsNoTracking().Where(p => p.STOrderId == order.Id && p.ItemId == item.OldItemId).FirstOrDefault();
                        //var rec = order.OrderedItems.Where(p => p.ItemId == item.OldItemId).FirstOrDefault();
                        rec.ItemId = item.ItemId;

                        rec.DateUpdated = DateTime.Now;


                        //will get deliveries to be updated
                        var clientDel = _context.STClientDeliveries.AsNoTracking().Where(p => p.STOrderDetailId == rec.Id && p.ItemId == item.OldItemId).ToList();
                        var srDel = _context.STShowroomDeliveries.AsNoTracking().Where(p => p.STOrderDetailId == rec.Id && p.ItemId == item.OldItemId).ToList();

                        // Update deliveries
                        if (srDel.Count > 0)
                        {
                            this.UpdateAffectedShowroomDeliveries(srDel, item, order.WarehouseId);
                        }
                        else
                        {
                            if (clientDel != null)
                            {
                                this.UpdateAffectedClientDeliveries(clientDel, item, order.WarehouseId);
                            }

                        }


                        //Getting stock details
                        var whStock = _context.WHStocks.AsNoTracking().Where(p => p.WarehouseId == order.WarehouseId && p.ItemId == item.ItemId).FirstOrDefault();
                        //var oldItemStock = _context.WHStocks.Where(p => p.WarehouseId == order.WarehouseId && p.ItemId == item.OldItemId 
                        //&& p.STClientDeliveryId == item.StClientDeliveryId 
                        //&& p.STShowroomDeliveryId == item.StShowroomDeliveryId).FirstOrDefault();

                        ////Update Change date to update stock summary
                        whStock.ChangeDate = DateTime.Now;

                        //oldItemStock.ChangeDate = DateTime.Now;
                        //oldItemStock.ItemId = item.ItemId;

                        item.RequestStatus = RequestStatusEnum.Approved;
                        item.DateUpdated = DateTime.Now;

                        _context.WHModifyItemTonalityDetails.Update(item);
                        _context.WHStocks.Update(whStock);
                        //_context.WHStocks.Update(oldItemStock);
                        _context.STOrderDetails.Update(rec);
                        //_context.SaveChanges();


                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }

                



            }

          





        }


        private void UpdateModifyTonality(WHModifyItemTonality record)
        {
            record.DateUpdated = DateTime.Now;
            record.RequestStatus = RequestStatusEnum.Approved;
            record.DateApproved = DateTime.Now;


            _context.WHModifyItemTonality.Update(record);
        }

        private void UpdateAffectedShowroomDeliveries(List<STShowroomDelivery> deliveries, WHModifyItemTonalityDetails item, int? WarehouseId)
        {

            foreach (var del in deliveries)
            {
                del.ItemId = item.ItemId;
                del.DateUpdated = DateTime.Now;

                var stDeliveryStock = _context.WHStocks.AsNoTracking().Where(p => p.WarehouseId == WarehouseId
                                                        && p.ItemId == item.OldItemId
                                                        && p.STShowroomDeliveryId == del.Id)
                                                        .FirstOrDefault();

                stDeliveryStock.ItemId = item.ItemId;
                stDeliveryStock.ChangeDate = DateTime.Now;

                _context.WHStocks.Update(stDeliveryStock);
                _context.STShowroomDeliveries.Update(del);


            }

        }
        private void UpdateAffectedClientDeliveries(List<STClientDelivery> deliveries, WHModifyItemTonalityDetails item, int? WarehouseId)
        {

            foreach (var del in deliveries)
            {
                del.ItemId = item.ItemId;
                del.DateUpdated = DateTime.Now;

                var clDeliveryStock = _context.WHStocks.AsNoTracking().Where(p => p.WarehouseId == WarehouseId
                                                    && p.ItemId == item.OldItemId
                                                    && p.STClientDeliveryId == del.Id)
                                                    .FirstOrDefault();

                clDeliveryStock.ItemId = item.ItemId;
                clDeliveryStock.ChangeDate = DateTime.Now;

                _context.WHStocks.Update(clDeliveryStock);
                _context.STClientDeliveries.Update(del);

            }
        }

    }

        
}
