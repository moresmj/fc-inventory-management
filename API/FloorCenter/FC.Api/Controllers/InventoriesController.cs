using FC.Api.DTOs.Inventories;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.StockHistory;
using FC.Api.Services.Warehouses;
using FC.Api.Services.Warehouses.StockHistory;
using FC.Core.Domain.Common;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{
    [Route("api/inventories")]
    public class InventoriesController : BaseController
    {
        private IWHStockService _warehouseStockService;
        private ISTStockService _storeStockService;
        private IWarehouseStockHistoryService _warehouseStockHistoryService;
        private IStoreStockHistoryService _storeStockHistoryService;
        private readonly AppSettings _appSettings;

        public InventoriesController(IWHStockService whStockService, ISTStockService stStockService,
                                     IWarehouseStockHistoryService warehouseStockHistoryService,
                                     IStoreStockHistoryService storeStockHistoryService, 
                                     IOptions<AppSettings> appSettings)
        {
            _warehouseStockService = whStockService;
            _storeStockService = stStockService;
            _warehouseStockHistoryService = warehouseStockHistoryService;
            _storeStockHistoryService = storeStockHistoryService;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        [Route("warehouse")]
        public IActionResult GetWarehouseInventories(InventorySearchDTO search)
        {
            search.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            //var list = _warehouseStockService.GetInventoriesByWarehouseId(search);
            var list = _warehouseStockService.GetInventoriesByWarehouseIdPaged(search, _appSettings);


            return Ok(list);
        }

        [HttpGet]
        [Route("release/details/{itemId}")]
        public IActionResult GetWarehouseItemReleaseDetails(int itemId, InventorySearchDTO search)
        {
            search.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());

            var list = _warehouseStockService.GetWarehouseForReleasingDetails(itemId, search, _appSettings);

            return Ok(list);
        }

        [HttpGet]
        [Route("STrelease/details/{itemId}")]
        public IActionResult GetStoreItemReleaseDetails(int itemId, InventorySearchDTO search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            search.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());

            var list = _warehouseStockService.GetStoreForReleasingDetails(itemId, search, _appSettings);

            return Ok(list);
        }
        [HttpGet]
        [Route("warehouse/reserveitems/{id}")]
        public IActionResult GetWarehouseItemsWithReserve(int id)
        {
            var WarehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseStockService.GetItemsWithReservation(WarehouseId);
            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/items/{id}")]
        public IActionResult GetWarehouseItems(int id)
        {
            var list = _warehouseStockService.GetWarehouseItemsByWarehouseId(id);
            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/items/code/{id}")]
        public IActionResult GetWarehouseItemsWithUniqueCode(int id)
        {
            var list = _warehouseStockService.GetWarehouseItemsByWarehouseIdWithUniqueCode(id);
            return Ok(list);
        }

        [HttpGet]
        [Route("store")]
        public IActionResult GetStoreInventories(InventorySearchDTO search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            //var list = _storeStockService.GetInventoriesByStoreId(search);
            var list = _storeStockService.GetInventoriesByStoreIdPaged(search, _appSettings);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/physicalcount/summary")]
        public IActionResult GetStorePhysicalCountSummary(InventorySearchDTO search)
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            //var list = _storeStockService.GetInventoriesSummaryByStoreId(storeId);

            var list = _storeStockService.GetInvetoriesSummaryByStoreIdPaged(storeId, search, _appSettings);
            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/physicalcount/summary")]
        public IActionResult GetWarehousePhysicalCountSummary(InventorySearchDTO search)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());

            //var list = _warehouseStockService.GetInventoriesSummaryByWarehouseId(warehouseId);
            var list = _warehouseStockService.GetInventoriesSummaryByWarehouseIdPaged(warehouseId, search, _appSettings);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/salesitem")]
        public IActionResult GetInventoriesByStoreIdForSalesFromSTStockSummary(InventorySearchDTO dto)
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            var list = _storeStockService.GetInventoriesByStoreIdForSalesFromSTStockSummary(storeId, dto);
            return Ok(list);
        }

        [HttpGet]
        [Route("store/salesitem/old")]  // Old process on getting the sales item
        public IActionResult GetStoreInventoriesForSales(InventorySearchDTO dto)
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            var list = _storeStockService.GetInventoriesByStoreIdForSales(storeId, dto);
            return Ok(list);
        }

        [HttpGet]
        [Route("store/salesitemdropdown")] 
        public IActionResult GetStoreInventoriesForSalesDropdownFromSTStockSummary(InventorySearchDTO dto)
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            var list = _storeStockService.GetInventoriesByStoreIdForSalesDropdownFromSTStockSummary(storeId, dto);
            return Ok(list);
        }

        [HttpGet]
        [Route("store/salesitemdropdown/old")]  // Old process on getting the sales item dropdown
        public IActionResult GetStoreInventoriesForSalesDropdown(InventorySearchDTO dto)
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            var list = _storeStockService.GetInventoriesByStoreIdForSalesDropdown(storeId, dto);
            return Ok(list);
        }


        [HttpGet]
        [Route("main/stores")]
        public IActionResult GetStoreInventoriesMain(InventorySearchDTO dto)
        {
            var list = _storeStockService.GetStoreInventories(dto);
            return Ok(list);
        }

        [HttpGet]
        [Route("main/warehouses")]
        public IActionResult GetWarehouseInventoriesMain(InventorySearchDTO dto)
        {
            var list = _warehouseStockService.GetWarehouseInventories(dto);
            return Ok(list);
        }


        [HttpGet]
        [Route("warehouse/stockhistory/{id}")]
        public IActionResult GetWarehouseStockHistory(int id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockHistoryService.DataContext());

            var list = _warehouseStockHistoryService.GetByItemIdAndWarehouseId(id, warehouseId);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/stockhistory/{id}")]
        public IActionResult GetStoreStockHistory(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_storeStockHistoryService.DataContext());

            var list = _storeStockHistoryService.GetByItemIdAndStoreId(id, storeId);

            return Ok(list);
        }


        [HttpGet]
        [Route("main/s_stockhistory")]
        public IActionResult GetMainSelectedStoreItemHistory(InventorySearchDTO dto)
        {

            var list = _storeStockHistoryService.GetMainSelectedStoreItemHistory(dto);
            return Ok(list);
        }

        [HttpGet]
        [Route("main/w_stockhistory")]
        public IActionResult GetMainSelectedWarehouseItemHistory(InventorySearchDTO dto)
        {

            var list = _warehouseStockHistoryService.GetMainSelectedWarehouseItemHistory(dto);
            return Ok(list);
        }


        [HttpGet]
        [Route("main/inventory-monitoring/incoming")]
        public IActionResult GetInventoryIncomingStockHistoryByStoreId(InventorySearchDTO dto)
        {

            //var list = _storeStockHistoryService.GetInventoryStockHistory(dto, InventoryMonitoringTypeEnum.Incoming);
            var list = _storeStockHistoryService.GetInventoryStockHistoryPaged(dto, InventoryMonitoringTypeEnum.Incoming, _appSettings);
            return Ok(list);
        }

        [HttpGet]
        [Route("main/inventory-monitoring/outgoing")]
        public IActionResult GetInventoryOutgoingStockHistoryByStoreId(InventorySearchDTO dto)
        {

            //var list = _storeStockHistoryService.GetInventoryStockHistory(dto, InventoryMonitoringTypeEnum.Outgoing);
            var list = _storeStockHistoryService.GetInventoryStockHistoryPaged(dto, InventoryMonitoringTypeEnum.Outgoing, _appSettings);
            return Ok(list);
        }

        [HttpGet] 
        [Route("store/interbranch")]
        public IActionResult GetStoresWithItem(InventorySearchDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_storeStockHistoryService.DataContext());
            dto.UserType = this.GetCurrentUserUserType(_storeStockHistoryService.DataContext());
            if(dto.UserType == UserTypeEnum.Dealer)
            {
                var user = this.GetUser(_storeStockHistoryService.DataContext());
                dto.UserId = user.Id;
            }

           

            var list = _storeStockService.GetStoresWithItem(dto);
            return Ok(list);
        }


        #region Old Functions


        [HttpGet]
        [Route("warehouse/old")]
        public IActionResult GetWarehouseInventoriesOld(InventorySearchDTO search)
        {
            search.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseStockService.GetInventoriesByWarehouseIdOld(search);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/old")]
        public IActionResult GetStoreInventoriesOld(InventorySearchDTO search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());

            var list = _storeStockService.GetInventoriesByStoreIdOld(search);

            return Ok(list);
        }

        #endregion

    }
}