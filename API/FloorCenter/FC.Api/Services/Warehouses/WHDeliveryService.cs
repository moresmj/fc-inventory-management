using System;
using System.Linq;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Warehouses;

namespace FC.Api.Services.Warehouses
{
    public class WHDeliveryService : IWHDeliveryService
    {

        private DataContext _context;

        public WHDeliveryService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public void InsertPurchaseReturnDelivery(WHDelivery dt)
        {
            dt.STReturnId = dt.Id;
            dt.Id = 0;
            dt.DateCreated = DateTime.Now;

            foreach (var delivery in dt.WarehouseDeliveries)
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
            }

            //  Filter only record with request deliver quantity more than 0
            dt.WarehouseDeliveries = dt.WarehouseDeliveries.Where(p => p.Quantity > 0).ToList();

            _context.WHDeliveries.Add(dt);
            _context.SaveChanges();
        }

    }
}
