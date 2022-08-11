using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Warehouses
{
    public class WHReceiveService : IWHReceiveService
    {


        private DataContext _context;

        public WHReceiveService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return _context;
        }
        
        /// <summary>
        /// Get all receives
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>WHReceives</returns>
        public IEnumerable<WHReceive> GetAllReceives(WHReceiveSearchDTO dto)
        {
            IQueryable<WHReceive> query = _context.WHReceives.Where(p => p.WarehouseId == dto.WarehouseId).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(dto.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == dto.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(dto.DRNumber))
            {
                query = query.Where(p => p.DRNumber.ToLower() == dto.DRNumber.ToLower());
            }

            if(dto.UserId.HasValue)
            {
                query = query.Where(p => p.UserId == dto.UserId);
            }

            if(dto.PODateFrom.HasValue)
            {
                query = query.Where(p => dto.PODateFrom.Value <= p.PODate);
            }

            if (dto.PODateTo.HasValue)
            {
                query = query.Where(p => dto.PODateTo.Value >= p.PODate);
            }


            if (dto.DRDateFrom.HasValue)
            {
                query = query.Where(p => dto.DRDateFrom.Value <= p.DRDate);
            }

            if (dto.DRDateTo.HasValue)
            {
                query = query.Where(p => dto.DRDateTo.Value >= p.DRDate);
            }


            if(!string.IsNullOrWhiteSpace(dto.ItemName))
            {
                query = query.Where(x => x.ReceivedItems.Where(k => k.Item.Name.Contains(dto.ItemName)).Count() > 0);
            }



            return query
                    .Include(p => p.ReceivedItems)
                              .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                    .Include(p => p.User);
        }


        public object GetAllReceives2(WHReceiveSearchDTO search, AppSettings appSettings)
        {

            IQueryable<WHReceive> query = _context.WHReceives.Where(p => p.WarehouseId == search.WarehouseId).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            if (search.UserId.HasValue)
            {
                query = query.Where(p => p.UserId == search.UserId);
            }

            if (search.PODateFrom.HasValue)
            {
                query = query.Where(p => search.PODateFrom.Value <= p.PODate);
            }

            if (search.PODateTo.HasValue)
            {
                query = query.Where(p => search.PODateTo.Value >= p.PODate);
            }


            if (search.DRDateFrom.HasValue)
            {
                query = query.Where(p => search.DRDateFrom.Value <= p.DRDate);
            }

            if (search.DRDateTo.HasValue)
            {
                query = query.Where(p => search.DRDateTo.Value >= p.DRDate);
            }


            if (!string.IsNullOrWhiteSpace(search.ItemName))
            {
                query = query.Where(x => x.ReceivedItems.Where(k => k.Item.Name.Contains(search.ItemName)).Count() > 0);
            }

            GetAllResponse response = null;


            if (search.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                query = query.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                        .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(query.Count());
            }

            query = query
                        .Include(p => p.ReceivedItems)
                                  .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                        .Include(p => p.User);

            response.List.AddRange(query);



            return response;
        }



       



        /// <summary>
        /// Get receive by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>WHReceive</returns>
        public WHReceive GetReceiveByIdAndWarehouseId(int? id, int? warehouseId)
        {
            return _context.WHReceives
                        .Where(p => p.WarehouseId == warehouseId && p.Id == id)
                            .Include(p => p.ReceivedItems)
                                .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                            .Include(p => p.User).FirstOrDefault();
        }

        /// <summary>
        /// Insert whreceive, whreceivedetail and whstock
        /// </summary>
        /// <param name="whReceive">WHReceive</param>
        /// <param name="_mapper">IMapper</param>
        /// <param name="wHStockService">WHStockService</param>
        public void InsertReceive(WHReceive whReceive, IMapper _mapper, IWHStockService wHStockService)
        {
            var totalRecordCount = Convert.ToInt32(this._context.WHReceives.Count() + 1).ToString();

            whReceive.TransactionNo = string.Format("P{0}", totalRecordCount.PadLeft(6, '0'));

            whReceive.DateCreated = DateTime.Now;
            _context.WHReceives.Add(whReceive);
            _context.SaveChanges();


            foreach(var item in whReceive.ReceivedItems)
            {

                item.DateCreated = DateTime.Now;
                _context.WHReceiveDetails.Update(item);
                _context.SaveChanges();

                var obj = new WHStock
                {
                    WarehouseId = whReceive.WarehouseId,
                    WHReceiveDetailId = item.Id,
                    ItemId = item.ItemId,
                    //client request reserved quantity will be deducted on the received qty
                    OnHand = item.Quantity - item.ReservedQuantity,
                    //added for advance order
                    Reserved = item.ReservedQuantity,
                    TransactionType = TransactionTypeEnum.PO,
                    DeliveryStatus = DeliveryStatusEnum.Delivered
                };

                wHStockService.InsertStock(obj);
            }

        }

    }
}
