using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FC.Api.Services.Stores
{
    public interface ISTOrderService
    {


        DataContext DataContext();

        /// <summary>
        /// Insert order
        /// </summary>
        /// <param name="stOrder">STOrder</param>
        void InsertOrder(STOrder stOrder, AppSettings appSettings);


        /// <summary>
        /// Insert order for interbranch
        /// </summary>
        /// <param name="stOrder">STOrder</param>
        void InsertOrderInterBranch(STOrder stOrder, ClaimsPrincipal user, AppSettings appSettings);


        void InsertAdvanceOrder(STAdvanceOrder sTAdvanceOrder);
        
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>STOrders</returns>
        IEnumerable<STOrder> GetAllOrders(OrderSearch dto);
        IEnumerable<object> GetAllOrders2(OrderSearch dto);


        object GetAllOrders3(OrderSearch search, AppSettings appSettings);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>STOrders</returns>
        IEnumerable<STOrder> GetAllOrders(SearchApproveRequests dto);

        IEnumerable<object> GetAllOrders2(SearchApproveRequests dto);

        object GetAllOrders3(SearchApproveRequests criteria, AppSettings appSettings);

        IEnumerable<object> GetOrdersToBeAssignedWithDRNumber(OrderSearch search);

        object GetOrdersToBeAssignedWithDRNumberPaged(OrderSearch search, AppSettings appSettings);

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrder</returns>
        STOrder GetOrderByIdAndStoreId(int? id, int? storeId);

        STOrder GetOrderById(int? id);


        /// <summary>
        /// Updates order used in Main site for Approve Request module
        /// </summary>
        /// <param name="obj">STOrder</param>
        /// <param name="whStockService">IWHStockService</param>
        void UpdateOrder(STOrder obj);

        void UpdateWHDRNumberOfSTOrder(STOrder param, IWHStockService whStockService);

        /// <summary>
        /// Get all for deliveries
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STOrders</returns>
        IEnumerable<STOrder> GetAllForDeliveries(SearchDeliveries search);


        /// <summary>
        /// Get all for receiving
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STOrders</returns>
        IEnumerable<STOrder> GetAllForReceiving(SearchReceiveItems search);
        


        /// <summary>
        /// Get receiving item
        /// </summary>
        /// <param name="id">STDelivery.Id</param>
        /// <returns>STOrder</returns>
        object GetReceivingItemByIdAndStoreId(int? id, int? storeId);

        STOrder GetReceivingItemById(int? id);


        /// <summary>
        /// Get showroom deliveries
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrders</returns>
        object GetShowroomDeliveriesByIdAndStoreId(int? id, int? storeId);
        


        /// <summary>
        /// Get delivery by id
        /// </summary>
        /// <param name="id">STDelivery.Id</param>
        /// <returns>STOrder</returns>
        STOrder GetDeliveryById(int? id);
        

        /// <summary>
        /// Get all for releasing
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STOrders</returns>
        IEnumerable<STOrder> GetAllForReleasing(SearchReleasing search);

        object GetAllForReleasingPaged(SearchReleasing search, AppSettings appSettings);

        object GetForShowroomReleasing(SearchReleasing search, AppSettings appSettings);

        object GetAllForTonalityReplacement(SearchReleasing search, AppSettings appSettings);

        void AddModifyItemTonality(WHModifyItemTonalityDTO record);


        /// <summary>
        /// Get for releasing by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrder</returns>
        STOrder GetReleasingById(int? id, int? warehouseId);


        void UpdateForReleasing(int? id, int? warehouseId);


        /// <summary>
        /// Update client order
        /// </summary>
        /// <param name="salesService"></param>
        /// <param name="id">STOrder.Id</param>
        void UpdateClientOrderForReleasing(STOrder record, ISTSalesService salesService, ISTStockService stockService, int id, int? warehouseId);


        /// <summary>
        /// Get client's order for releasing by id
        /// </summary>
        /// <param name="id">STOrder.Id</param>
        /// <returns>STOrder</returns>
        STOrder GetClientOrderReleasingByIdAndWarehouseId(int? id, int? warehouseId);


        object GetClientDeliveriesByIdAndStoreId(int? id, int? storeId);


        IQueryable<STOrder> DrnumberQuery(OrderSearch search);

        //assign client si number
        void AddClientSINumber(AddClientSIDTO dto, int? storeId);


        object GetAllForReceiving2(SearchReceiveItems search);

        void InsertDeliveryToShowroom(STOrder order, int storeId);

        //Get advance order for approve on main
        object GetAllAdvanceOrders(SearchApproveRequests search, AppSettings appSettings);

    }
}
