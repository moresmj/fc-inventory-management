// MODULES
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { HttpModule } from "@angular/http";
import { MainRoutingModule } from "@modules/main/main-routing.module";
import { RouterModule } from "@angular/router";
import { SharedModule } from "@modules/shared.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { AuthGuard } from "@library/_guards/auth.guard";

import { UiSwitchModule } from "ngx-toggle-switch";
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
// FILTERS

// VALIDATORS

// SERVICES
import { PageModuleService } from "@services/common/pageModule.service";
import { BaseService } from "@services/base.service";
import { RequestService } from "@services/request.service";
import { CommonViewService } from "@services/common/common-view.service";
import { PagerService } from "@services/common/pager.service";
import { UserService } from "@services/user/user.service";
import { StoreService } from "@services/store/store.service";
import { WarehouseService } from "@services/warehouse/warehouse.service";
import { ItemService } from "@services/item/item.service";
import { ApproveRequestsService } from "@services/approve-requests/approve-requests.service";
import { ReceiveItemsService } from "@services/transactions/receive-items.service";
import { OrderStockService } from "@services/order-stock/order-stock.service";
import { OrderService } from "@services/order/order.service";
import { DeliveriesService } from "@services/deliveries/deliveries.service";
import { ReleaseItemsService } from "@services/release-item/release-items.service";
import { ReleasingService } from "@services/releasing/releasing.service";
import { InventoriesService } from "@services/inventories/inventories.service";
import { SalesOrderService } from "@services/sales-order/sales-order.service";
import { BranchOrderService } from "@services/branch-order/branch-order.service";
import { ClientReleasingService } from "@services/releasing/client-releasing.service";
import { SalesOrderReleasingService } from "@services/releasing/sales-order-releasing.service";
import { SameDaySalesReleasingService } from "@services/releasing/same-day-sales-releasing.service";
import { TransferReleasingService } from "@services/releasing/transfer-releasing.service";
import { ApiBaseService } from "@services/api-base.service";
import { LoadingIndicatorService } from "@services/common/loader.service";

// COMPONENTS
import { MainBaseComponent } from "@baseComponents/main/main-base.component";
import { DashboardComponent } from "@main/dashboard/dashboard.component";
import { UserListComponent } from "@m_transaction/user/list/user-list.component";
import { UserAddComponent } from "@m_transaction/user/add/user-add.component";
import { UserUpdateComponent } from "@m_transaction/user/update/user-update.component";
import { UserDealerListComponent } from "@m_transaction/user-dealer/list/user-dealer-list.component";
import { UserDealerAddComponent } from "@m_transaction/user-dealer/add/user-dealer-add.component";
import { UserDealerUpdateComponent } from "@m_transaction/user-dealer/update/user-dealer-update.component";
import { ItemListComponent } from "@m_transaction/item/list/item-list.component";
import { ItemAddComponent } from "@m_transaction/item/add/item-add.component";
import { ItemUpdateComponent } from "@m_transaction/item/update/item-update.component";
import { StoreListComponent } from "@main/transaction/store/list/store-list.component";
import { StoreAddComponent } from "@main/transaction/store/add/store-add.component";
import { StoreUpdateComponent } from "@main/transaction/store/update/store-update.component";
import { WarehouseListComponent } from "@main/transaction/warehouse/list/warehouse-list.component";
import { WarehouseAddComponent } from "@main/transaction/warehouse/add/warehouse-add.component";
import { WarehouseUpdateComponent } from "@main/transaction/warehouse/update/warehouse-update.component";
import { ApproveRequestOrdersListComponent } from "@m_transaction/approve-requests/orders/list/ar-orders-list.component";
import { ApproveRequestOrdersDetailsComponent } from "@m_transaction/approve-requests/orders/details/ar-orders-details.component";
import { ApproveRequestOrdersAdvancedSearchComponent } from "@m_transaction/approve-requests/orders/advanced-search/ar-orders-advanced-search.component";
import { ApproveRequestReturnsListComponent } from "@m_transaction/approve-requests/returns/list/ar-returns-list.component";
import { ApproveRequestReturnsDetailsComponent } from "@m_transaction/approve-requests/returns/details/ar-returns-details.component";
import { ApproveRequestReturnsAdvancedSearchComponent } from "@m_transaction/approve-requests/returns/advanced-search/ar-returns-advanced-search.component";
import { ApproveRequestTransfersListComponent } from "@m_transaction/approve-requests/transfers/list/ar-transfers-list.component";
import { ApproveRequestTransfersDetailsComponent } from "@m_transaction/approve-requests/transfers/details/ar-transfers-details.component";
import { ApproveRequestTransfersAdvancedSearchComponent } from "@m_transaction/approve-requests/transfers/advanced-search/ar-transfers-advanced-search.component";
import { StoreInventoryListComponent } from "@m_im/store-inventory/list/si-list.component";
import { StoreInventoryDetailsComponent } from "@m_im/store-inventory/details/si-details.component";
import { StoreInventoryAdvancedSearchComponent } from "@m_im/store-inventory/advanced-search/si-advanced-search.component";
import { WarehouseInventoryListComponent } from "@m_im/warehouse-inventory/list/wi-list.component";
import { WarehouseInventoryDetailsComponent } from "@m_im/warehouse-inventory/details/wi-details.component";
import { WarehouseInventoryAdvancedSearchComponent } from "@m_im/warehouse-inventory/advanced-search/wi-advanced-search.component";

import { StoreStockHistoryComponent } from "@m_im/store-inventory/_stock-history/s-stock-history.component";
import { WarehouseStockHistoryComponent } from "@m_im/warehouse-inventory/_stock-history/w-stock-history.component";

import { AdjustInventoryListComponent } from "@m_transaction/adjust-inventory/list/ai-list.component";
import { AdjustInventoryDetailsComponent } from "@m_transaction/adjust-inventory/_approve/ai-details.component";
import { ApproveAdjustInventoryAdvancedSearchComponent } from "@m_transaction/adjust-inventory/advanced-search/ai-advanced-search.component";

import { AdjustWarehouseInventoryListComponent } from "@m_transaction/adjust-inventory/warehouse/list/w-ai-list.component";
import { AdjustWarehouseInventoryDetailsComponent } from "@m_transaction/adjust-inventory/warehouse/_approve/w-ai-details.component";

import { InventoryMonitoringListComponent } from "@m_reports/im-incoming/list/im-incoming-list.component";
import { InventoryMonitoringAdvancedSearchComponent } from "@m_reports/im-incoming/advanced-search/im-incoming-advanced-search.component";
import { InventoryMonitoringOutgoingListComponent } from "@m_reports/im-outgoing/list/im-outgoing-list.component";
import { InventoryMonitoringOutgoingAdvancedSearchComponent } from "@m_reports/im-outgoing/advanced-search/im-outgoing-advanced-search.component";

import { AssignDrListComponent } from "@m_transaction/assign-dr/list/assign-dr-list.component";
import { AssignDrDetailsComponent } from "@m_transaction/assign-dr/details/assign-dr-details.component";
import { AssignDrAdvancedSearchComponent } from "@m_transaction/assign-dr/advanced-search/assign-dr-advanced-search.component";

import { AddSizeComponent } from "@m_transaction/item/add/add-size/add-size.component";

import { AuditTrailListComponent } from "@main/audit-trail/list/audit-trail-list.component";
import { StoreRTVListComponent } from "@m_reports/store-rtv/list/s-rtv-list.component";
import { StoreRTVAdvancedSearchComponent } from "@m_reports/store-rtv/advanced-search/s-rtv-advanced-search.component";

import { CompanyListComponent } from "@m_transaction/company/list/company-list.component";
import { CompanyAddComponent } from "@m_transaction/company/add/company-add.component";
import { CompanyUpdateComponent } from "@m_transaction/company/update/company-update.component";

import { ApproveRequestChangeTonalityListComponent } from "@m_transaction/approve-requests/change-tonality/list/change-tonality-list.component";
import { ApproveRequestChangeTonalityAdvancedSearchComponent } from "@m_transaction/approve-requests/change-tonality/advanced-search/change-tonality-advanced-search.component";
import { ApproveRequestChangeTonalityDetailsComponent } from "@m_transaction/approve-requests/change-tonality/details/change-tonality-details.component";


import { ApproveAdvanceOrderListComponent } from "@m_transaction/approve-requests/advance-orders/list/advance-orders-list.component";
import { ApproveRequestAdvanceOrderAdvancedSearchComponent } from "@m_transaction/approve-requests/advance-orders/advanced-search/advance-orders-advanced-search.component";
import { ApproveRequestAdvanceOrderDetailsComponent } from "@m_transaction/approve-requests/advance-orders/details/advance-orders-details.component";

import { ApproveRequestOrdersDealerListComponent } from "@m_transaction/approve-requests/orders-dealer/list/ar-orders-dealer-list.component";
import { ApproveRequestOrdersDealersDetailsComponent } from "@m_transaction/approve-requests/orders-dealer/details/ar-orders-dealer-details.component";
import { ApproveRequestOrdersDealerAdvancedSearchComponent } from "@m_transaction/approve-requests/orders-dealer/advanced-search/ar-orders-dealer-advanced-search.component";

@NgModule({
  imports: [
    CommonModule,
    MainRoutingModule,
    RouterModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    UiSwitchModule,
    NgMultiSelectDropDownModule.forRoot()
  ],
  declarations: [
    MainBaseComponent,
    DashboardComponent,
    UserListComponent,
    UserAddComponent,
    UserUpdateComponent,
    UserDealerListComponent,
    UserDealerAddComponent,
    UserDealerUpdateComponent,
    StoreListComponent,
    StoreAddComponent,
    StoreUpdateComponent,
    WarehouseListComponent,
    WarehouseAddComponent,
    WarehouseUpdateComponent,
    ItemListComponent,
    ItemAddComponent,
    ItemUpdateComponent,
    ApproveRequestOrdersListComponent,
    ApproveRequestOrdersDetailsComponent,
    ApproveRequestOrdersAdvancedSearchComponent,
    ApproveRequestReturnsListComponent,
    ApproveRequestReturnsDetailsComponent,
    ApproveRequestReturnsAdvancedSearchComponent,
    ApproveRequestTransfersListComponent,
    ApproveRequestTransfersDetailsComponent,
    ApproveRequestTransfersAdvancedSearchComponent,
    StoreInventoryListComponent,
    StoreInventoryDetailsComponent,
    StoreInventoryAdvancedSearchComponent,
    WarehouseInventoryListComponent,
    WarehouseInventoryDetailsComponent,
    WarehouseInventoryAdvancedSearchComponent,
    StoreStockHistoryComponent,
    WarehouseStockHistoryComponent,
    AdjustInventoryListComponent,
    AdjustInventoryDetailsComponent,
    ApproveAdjustInventoryAdvancedSearchComponent,
    AdjustWarehouseInventoryListComponent,
    AdjustWarehouseInventoryDetailsComponent,
    InventoryMonitoringListComponent,
    InventoryMonitoringAdvancedSearchComponent,
    InventoryMonitoringOutgoingListComponent,
    InventoryMonitoringOutgoingAdvancedSearchComponent,
    AssignDrListComponent,
    AssignDrDetailsComponent,
    AssignDrAdvancedSearchComponent,
    AddSizeComponent,
    AuditTrailListComponent,
    StoreRTVListComponent,
    StoreRTVAdvancedSearchComponent,
    CompanyListComponent,
    CompanyAddComponent,
    CompanyUpdateComponent,
    ApproveRequestChangeTonalityListComponent,
    ApproveRequestChangeTonalityAdvancedSearchComponent,
    ApproveRequestChangeTonalityDetailsComponent,
    ApproveAdvanceOrderListComponent,
    ApproveRequestAdvanceOrderAdvancedSearchComponent,
    ApproveRequestAdvanceOrderDetailsComponent,
    ApproveRequestOrdersDealerListComponent,
    ApproveRequestOrdersDealersDetailsComponent,
    ApproveRequestOrdersDealerAdvancedSearchComponent,
  ],
  providers: [
    AuthGuard,
    PageModuleService,
    BaseService,
    RequestService,
    CommonViewService,
    PagerService,
    UserService,
    StoreService,
    WarehouseService,
    ItemService,
    ApproveRequestsService,
    ReceiveItemsService,
    OrderStockService,
    OrderService,
    DeliveriesService,
    ReleaseItemsService,
    ReleasingService,
    InventoriesService,
    SalesOrderService,
    BranchOrderService,
    ClientReleasingService,
    SalesOrderReleasingService,
    SameDaySalesReleasingService,
    TransferReleasingService,
    ApiBaseService,
    LoadingIndicatorService
  ],
  exports: [ItemListComponent, ItemAddComponent, ItemUpdateComponent]
})
export class MainModule {}
