using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;

namespace FC.Api.Services.Stores.PhysicalCount
{
    public interface IUploadPhysicalCountService
    {

        DataContext DataContext();

        void SavePhysicalCount(STImport param);

        string SavePhysicalCount2(STImport param);

        string SaveBreakage(STImport param);

        IEnumerable<object> GetAll(ApprovePhysicalCountSearchDTO search);

        object GetById(int id);

        int GetById2(int id);

        void Approve(ApproveStorePhysicalCountDTO dto, ISTStockService stStockService);

        void ApproveBreakage(ApproveStorePhysicalCountDTO dto, ISTStockService stStockService);

        ApproveStorePhysicalCountDTO GetByTransNo(string transNo);
    }
}
