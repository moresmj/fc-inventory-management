using AutoMapper;
using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Core.Domain.Warehouses;
using System.Collections.Generic;

namespace FC.Api.Services.Warehouses
{
    public interface IWHReceiveService
    {


        DataContext DataContext();

        /// <summary>
        /// Insert whreceive, whreceivedetail and whstock
        /// </summary>
        /// <param name="whReceive">WHReceive</param>
        /// <param name="_mapper">IMapper</param>
        /// <param name="wHStockService">WHStockService</param>
        void InsertReceive(WHReceive whReceive, IMapper _mapper, IWHStockService wHStockService);


        /// <summary>
        /// Get all receives
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>WHReceives</returns>
        IEnumerable<WHReceive> GetAllReceives(WHReceiveSearchDTO dto);


        object GetAllReceives2(WHReceiveSearchDTO search, AppSettings appSettings);


        /// <summary>
        /// Get receive by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>WHReceive</returns>
        WHReceive GetReceiveByIdAndWarehouseId(int? id, int? warehouseId);


    }
}
