using FC.Core.Domain.Warehouses;
using System.Collections.Generic;

namespace FC.Api.Services.Warehouses
{
    public interface IWarehouseService
    {

        /// <summary>
        /// Insert a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        void InsertWarehouse(Warehouse warehouse);


        /// <summary>
        /// Get all warehouses
        /// </summary>
        /// <returns>Warehouse List</returns>
        IEnumerable<Warehouse> GetAllWarehouses();

        /// <summary>
        /// Get  warehouses with param
        /// </summary>
        /// <returns>Warehouse List</returns>
        IEnumerable<Warehouse> GetWarehouseWithParam(int? id);


        /// <summary>
        /// Get warehouse by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Warehouse</returns>
        Warehouse GetWarehouseById(int? id);


        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        void UpdateWarehouse(Warehouse warehouse);

    }
}
