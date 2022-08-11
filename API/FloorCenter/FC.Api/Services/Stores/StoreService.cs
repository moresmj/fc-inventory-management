using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores
{
    public class StoreService : IStoreService
    {

        private DataContext _context;

        public StoreService(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Insert a store
        /// </summary>
        /// <param name="store">Store</param>
        public void InsertStore(Store store)
        {
            store.DateCreated = DateTime.Now;

            _context.Stores.Add(store);
            _context.SaveChanges();
        }


        /// <summary>
        /// Get all stores
        /// </summary>
        /// <returns>Store List</returns>
        public IEnumerable<Store> GetAllStores()
        {
            return _context.Stores.AsNoTracking()
                        .Include(y => y.Company)
                        .Include(y => y.Warehouse).OrderBy(p => p.Name);
        }


        /// <summary>
        /// Get store by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Store</returns>
        public Store GetStoreById(int? id)
        {
            return _context.Stores
                        .Include(y => y.Company)
                        .Include(y => y.Warehouse)
                        .Where(p => p.Id == id)
                        .FirstOrDefault();
        }


        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="storeParam">Store</param>
        public void UpdateStore(Store storeParam)
        {
            var store = _context.Stores.Where(x => x.Id == storeParam.Id).SingleOrDefault();

            store.Name = storeParam.Name;
            store.Address = storeParam.Address;
            store.ContactNumber = storeParam.ContactNumber;
            store.CompanyId = storeParam.CompanyId;
            store.WarehouseId = storeParam.WarehouseId;
            store.DateUpdated = DateTime.Now;

            _context.Stores.Update(store);
            _context.SaveChanges();
        }

        public void InsertStoresHandled(StoreDealerAssignment assign)
        {
          

            _context.StoreDealerAssignment.Add(assign);
            _context.SaveChanges();
        }

    }
}
