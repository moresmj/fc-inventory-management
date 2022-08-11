using FC.Api.Helpers;
using FC.Core.Domain.Warehouses;

namespace FC.Api.Services.Warehouses
{
    public interface IWHDeliveryService
    {

        DataContext DataContext();

        void InsertPurchaseReturnDelivery(WHDelivery dt);
        
    }
}
