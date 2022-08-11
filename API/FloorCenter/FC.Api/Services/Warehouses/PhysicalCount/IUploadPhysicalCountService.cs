using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Warehouses.PhysicalCount
{
    public interface IUploadPhysicalCountService
    {

        DataContext DataContext();

        void SavePhysicalCount(WHImport param);

        string SavePhysicalCount2(WHImport param);

        string SavePhysicalCount3(WHImport param);

        string SaveReserveAdjustment(AdjustReservedItemQuantity param, IWHStockService whStockService);

        IEnumerable<object> GetAll();

        object GetById(int id);

        int GetById2(int id);

        void Approve(ApproveWarehousePhysicalCountDTO dto, IWHStockService whStockService);


        ApproveWarehousePhysicalCountDTO GetByTransNo(string transNo);

    }
}
