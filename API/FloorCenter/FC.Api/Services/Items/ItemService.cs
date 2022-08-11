using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FC.Core.Domain.Items;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Items
{
    public class ItemService : IItemService
    {

        private DataContext _context;

        public ItemService(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Insert an item
        /// </summary>
        /// <param name="item">Item</param>
        public void InsertItem(Item item)
        {
            item.DateCreated = DateTime.Now;
            item.IsActive = true;

            if (item.ItemAttribute != null)
            {
                item.ItemAttribute.DateCreated = DateTime.Now;
            }

            _context.Items.Add(item);
            _context.SaveChanges();
        }


        /// <summary>
        /// Get all items
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>Items</returns>
        public IEnumerable<Item> GetAllItems(ItemSearchDTO dto)
        {
            IQueryable<Item> query = _context.Items.AsNoTracking().OrderByDescending(p => p.Id);

            if(dto.Id > 0)
            {
                query = query.Where(p => p.Id == dto.Id);
            }

            if(!string.IsNullOrWhiteSpace(dto.Code))
            {
                query = query.Where(p => p.Code.ToLower() == dto.Code.ToLower());
            }

            if(!string.IsNullOrEmpty(dto.SerialNumber))
            {
                query = query.Where(p => p.SerialNumber == dto.SerialNumber);
            }

            if (!string.IsNullOrWhiteSpace(dto.Tonality))
            {
                query = query.Where(p => p.Tonality.ToLower() == dto.Tonality.ToLower());
            }

            if (dto.SizeId.HasValue)
            {
                query = query.Where(p => p.SizeId == dto.SizeId);
            }



            return query
                        .Include(p => p.Size);
        }



        /// <summary>
        /// Get all items
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>Items</returns>
        public object GetAllItems2(ItemSearchDTO dto,AppSettings appSettings)
        {
            IQueryable<Item> query = _context.Items.AsNoTracking()
                                        .OrderByDescending(p => p.Id)
                                            .Include(p => p.Size);

            if(!string.IsNullOrEmpty(dto.Keyword))
            {
                query = query.Where(p =>

                p.Code.Contains(dto.Keyword) ||

                p.Name.Contains(dto.Keyword) ||

                p.SerialNumber.Contains(dto.Keyword) ||

                p.SerialNumber.Contains(dto.Keyword) ||

                p.Tonality.Contains(dto.Keyword) ||

                p.Size.Name.Contains(dto.Keyword) ||
                
                p.Description.Contains(dto.Keyword));
            }


            GetAllResponse response = null;


            if (dto.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), dto.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if(dto.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();

             
                }

                query = query.Skip((dto.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(query.Count());
            }

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.ImageName,
                              x.SizeId,
                              x.Code,
                              x.SerialNumber,
                              x.Name,
                              x.Description,
                              x.Tonality,
                              x.SRP,
                              x.Cost,
                              x.Remarks,
                              x.QtyPerBox,
                              x.BoxPerPallet,
                              x.Size,
                              x.ItemType,
                              x.IsActive,
                              HasStock = (
                              ((_context.WHStockSummary.Where(p => p.ItemId == x.Id).Select(p => p.OnHand).Sum() > 0)
                              || (_context.STStockSummary.Where(p => p.ItemId == x.Id).Select(p => p.OnHand).Sum()) > 0)),
                          };

            records.ToList();
   

            response.List.AddRange(records);



            return response;
        }

        /// <summary>
        /// Get all items
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>Items</returns>
        public List<Item> GetAllItemsForDropDown()
        {
            List<Item> query = _context.Items.Where(p => p.IsActive == true).AsNoTracking().OrderByDescending(p => p.Id)
                                .Include(p => p.Size).ToList();/*.AsNoTracking()*/
                                                               /*.Include(p => p.ItemAttribute)*//*.AsNoTracking()*/




            return query;
        }


        /// <summary>
        /// Updates the item
        /// </summary>
        /// <param name="itemParam">Item</param>
        public void UpdateItem(Item itemParam)
        {
            var item = _context.Items.Where(x => x.Id == itemParam.Id).SingleOrDefault();

            item.Name = itemParam.Name;
            item.SerialNumber = itemParam.SerialNumber;
            item.Code = itemParam.Code;
            item.SRP = itemParam.SRP;
            item.Description = itemParam.Description;
            item.SizeId = itemParam.SizeId;
            item.ImageName = itemParam.ImageName;
            item.Tonality = itemParam.Tonality;
            item.CategoryParentId = itemParam.CategoryParentId;
            item.CategoryChildId = itemParam.CategoryChildId;
            item.CategoryGrandChildId = itemParam.CategoryGrandChildId;
            item.QtyPerBox = itemParam.QtyPerBox;
            item.BoxPerPallet= itemParam.BoxPerPallet;
            item.DateUpdated = DateTime.Now;
            item.Cost = itemParam.Cost;
            item.IsActive = itemParam.IsActive;

            _context.Items.Update(item);
            _context.SaveChanges();
        }


    }
}
