using FC.Api.Helpers;
using FC.Core.Domain.Stores;

namespace FC.Api.Services.Stores.Returns
{
    public interface IPurchaseReturnService
    {

        DataContext DataContext();

        void AddPurchaseReturn(STReturn param);

        void AddBreakage(STReturn param);

        object GetPurchaseReturnByIdAndStoreId(int id, int? storeId);

        object GetPurchaseReturnDeliveryRecordsByIdAndStoreId(int? id, int? storeId);

    }
}
