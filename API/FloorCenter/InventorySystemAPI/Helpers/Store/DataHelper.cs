using InventorySystemAPI.Classes;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventorySystemAPI.Helpers.Store
{
    public class DataHelper : IDataHelper<STSales>
    {

        private readonly FloorCenterContext _context;

        private readonly Common _common;

        public DataHelper(FloorCenterContext context)
        {
            this._context = context;
            this._common = new Common();
        }

        public IQueryable<STSales> GetAll(DeliveryStatusEnum orderedItemsDeliveryStatus)
        {
            return (from x in _context.STSales
                           //.Where(x => x.StoreId == store_id_here)
                           .Include(y => y.OrderedItems)
                    select new STSales
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        SIPONumber = x.SIPONumber,
                        SIPODate = x.SIPODate,
                        CustomerName = x.CustomerName,
                        PaymentType = x.PaymentType,
                        DeliveryType = x.DeliveryType,
                        Remarks = x.Remarks,
                        OrderedItems = x.OrderedItems.Where(y => y.DeliveryStatus == orderedItemsDeliveryStatus).ToList(),
                    }).Where(x => x.OrderedItems.Count > 0);
        }

        public IQueryable<STSales> GetAll(BaseSearch search)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(search);

            var records = (from x in _context.STSales
                                .Where(x => _common.filterModelEqual(x, criteria))
                                .Include(y => y.OrderedItems)
                                    .ThenInclude(z => z.ReleasedItems)
                           select new STSales
                           {
                               Id = x.Id,
                               StoreId = x.StoreId,
                               SIPONumber = x.SIPONumber,
                               SIPODate = x.SIPODate,
                               CustomerName = x.CustomerName,
                               PaymentType = x.PaymentType,
                               DeliveryType = x.DeliveryType,
                               Remarks = x.Remarks,
                               OrderedItems = x.OrderedItems,
                           });


            return records;
        }

        public STSales GetSingle(BaseSearch search, DeliveryStatusEnum orderedItemsDeliveryStatus)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(search);

            var obj = (from x in _context.STSales
                        .Where(x => _common.filterModelEqual(x, criteria))
                        .Include(y => y.OrderedItems)
                       select new STSales
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           SIPONumber = x.SIPONumber,
                           SIPODate = x.SIPODate,
                           CustomerName = x.CustomerName,
                           CustomerAddress1 = x.CustomerAddress1,
                           CustomerAddress2 = x.CustomerAddress2,
                           CustomerAddress3 = x.CustomerAddress3,
                           PaymentType = x.PaymentType,
                           DeliveryType = x.DeliveryType,
                           Agent = x.Agent,
                           Remarks = x.Remarks,
                           OrderedItems = x.OrderedItems.Where(y => y.DeliveryStatus == orderedItemsDeliveryStatus).ToList(),
                       })
                        .Where(x => x.OrderedItems.Count > 0)
                        .FirstOrDefault();

            return obj;
        }
    }
}
