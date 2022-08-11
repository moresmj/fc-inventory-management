using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.Helpers;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Warehouses
{
    public class WarehouseService : IWarehouseService
    {

        private DataContext _context;

        public WarehouseService(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Insert a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public void InsertWarehouse(Warehouse warehouse)
        {
            warehouse.DateCreated = DateTime.Now;

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
        }


        /// <summary>
        /// Get all warehouses
        /// </summary>
        /// <returns>Warehouse List</returns>
        public IEnumerable<Warehouse> GetAllWarehouses()
        {
            return _context.Warehouses.AsNoTracking().OrderByDescending(p => p.Id);
        }


        /// <summary>
        /// Get all warehouses
        /// </summary>
        /// <returns>Warehouse List</returns>
        public IEnumerable<Warehouse> GetWarehouseWithParam(int? storeId)
        {
           var store = _context.Stores.Where(p => p.Id == storeId).FirstOrDefault();
            //var warehouse = _context.Warehouses.Where(p => p.Id == store.WarehouseId || p.Vendor == true).AsNoTracking().OrderByDescending(p => p.Id);
            var warehouse = _context.Warehouses.AsNoTracking().OrderBy(p => p.Id);
            return warehouse;
        }


        /// <summary>
        /// Get warehouse by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Warehouse</returns>
        public Warehouse GetWarehouseById(int? id)
        {
            return _context.Warehouses.Find(id);
        }


        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouseParam">Warehouse</param>
        public void UpdateWarehouse(Warehouse warehouseParam)
        {
            var warehouse = _context.Warehouses.Where(x => x.Id == warehouseParam.Id).SingleOrDefault();

            warehouse.Name = warehouseParam.Name;
            warehouse.Address = warehouseParam.Address;
            warehouse.ContactNumber = warehouseParam.ContactNumber;
            warehouse.DateUpdated = DateTime.Now;

            _context.Warehouses.Update(warehouse);
            _context.SaveChanges();
        }
    }
}
