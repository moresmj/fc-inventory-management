using InventorySystemAPI.Classes;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.Warehouse.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystemAPI.Helpers
{
    public interface IDataHelper<T>
    {

        IQueryable<T> GetAll(BaseSearch search);

        IQueryable<T> GetAll(DeliveryStatusEnum orderedItemsDeliveryStatus);

        T GetSingle(BaseSearch search, DeliveryStatusEnum orderedItemsDeliveryStatus);

    }
}
