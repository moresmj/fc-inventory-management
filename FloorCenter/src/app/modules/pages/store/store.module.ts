// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { StoreRoutingModule } from '@pageRoutes/store/store-routing.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@modules/pages/shared/shared.module';


// SERVICES
import { BaseService } from '@services/base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { PagerService } from '@services/common/pager.service';
import { ItemService } from '@services/item/item.service';
import { ProductService } from '@services/product/product.service';
import { StockService } from '@services/stock/stock.service';
import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { UserService } from '@services/user/user.service';
import { OrderRequestService } from '@services/order_request/order-request.service';
import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreRequestService } from '@services/store-request/store-request.service';
import { WarehouseInventoriesService } from '@services/warehouse/warehouse-inventories.service';



// FILTERS


// COMPONENTS
import { StoreBaseComponent } from '@baseComponents/store/store-base.component';
import { DashboardComponent } from '@store/Dashboard/dashboard.component';
import { ReceiveFromTransferListComponent } from '@s_srm/receive-from-transfer/list/rft-list.component';
import { ReceiveFromTransferUpdateComponent } from '@s_srm/receive-from-transfer/update/rft-update.component';
import { OutgoingListComponent } from '@s_stm/outgoing-list/list/og-list.component';
import { OutgoingListDetailsComponent } from '@s_stm/outgoing-list/details/og-list-details.component';
import { RequestRegistrationComponent } from '@s_stm/request-registration/rr.component';
import { SalesReleasingRegistrationComponent} from '@s_sar/registration/sl-registration.component';
import { ReturnRegistrationComponent } from '@s_rb/registration/return-registration.component';
import { ReturnBreakageListComponent } from '@s_rb/return-breakage/list/rb-list.component';
import { ReturnBreakageDetailsComponent } from '@s_rb/return-breakage/details/rb-details.component';
import { ReleasedRegistrationComponent } from '@s_sar/released-registration/rel-registration.component';
import { RequestListComponent } from '@s_drm/request-list/list/request-list.component';
import { RequestAdvancedSearchComponent } from '@s_drm/request-list/advanced-search/request-advanced-search.component';
import { RequestDetailsComponent } from '@s_drm/request-list/details/dr-details.component';
import { RequestUpdateComponent } from '@s_drm/request-list/details/update/dr-update.component';
import { RequestStoreDeliveryAddComponent } from '@s_drm/request-list/details/store-delivery-add/dr-sd-add.component';
import { RequestWarehouseDeliveryAddComponent } from '@s_drm/request-list/details/warehouse-delivery-add/dr-wd-add.component';
import { StockRequestRegistrationComponent } from '@s_stock_req/registration/sr-registration.component';
import { ReceiveFromIncomingListComponent } from '@s_stock_rec/receive-from-incoming/list/rfi-list.component';
import { ReceiveFromIncomingDetailsComponent } from '@s_stock_rec/receive-from-incoming/receive/details/rfi-details.component';
import { ReceiveListComponent } from '@s_stock_rec/receive-list/list/rl-list.component';
import { ReceiveListDetailsComponent } from '@s_stock_rec/receive-list/details/rl-details.component';
import { ReceiveListAdvancedSearchComponent } from '@s_stock_rec/receive-list/advanced-search/rl-advanced-search.component';

import { ReleasedListComponent } from '@s_sar/released-list/list/released-list.component';
import { ReleasedListDetailsComponent } from '@s_sar/released-list/details/released-list-details.component';
import { ReleasedListAdvancedSearchComponent } from '@s_sar/released-list/advanced-search/released-list-advanced-search.component';
import { DeliveryRequestListComponent } from '@s_sar/released-list/_delivery/list/delivery-request-list.component';
import { DeliveryRequestStoreDeliveryAddComponent } from '@s_sar/released-list/_delivery/store-delivery-add/dr-store-delivery-add.component';
import { DeliveryRequestUpdateStoreComponent } from '@s_sar/released-list/_delivery/update-store/dr-update-store.component';
import { DeliveryRequestWarehouseDeliveryAddComponent } from '@s_sar/released-list/_delivery/warehouse-delivery-add/dr-warehouse-delivery-add.component';
import { DeliveryRequestUpdateWarehouseComponent } from '@s_sar/released-list/_delivery/update-warehouse/dr-update-warehouse.component';


import { OrderTransferListComponent } from '@s_stock_req/order-transfer-list/list/order-transfer-list.component';
import { OrderTransferDetailsComponent } from '@s_stock_req/order-transfer-list/details/order-transfer-details.component';
import { OrderTransferAdvancedSearchComponent } from '@s_stock_req/order-transfer-list/advanced-search/order-transfer-advanced-search.component';
import { OrderTransferDeliveryRequestListComponent } from '@s_stock_req/order-transfer-list/_delivery/list/ot-delivery-request-list.component';
import { OrderTransferDeliveryRequestUpdateStoreComponent } from '@s_stock_req/order-transfer-list/_delivery/update-store/ot-dr-update-store.component';
import { OrderTransferDeliveryRequestUpdateWarehouseComponent } from '@s_stock_req/order-transfer-list/_delivery/update-warehouse/ot-dr-update-warehouse.component';
import { OrderTransferDeliveryRequestWADComponent } from '@s_stock_req/order-transfer-list/_delivery/warehouse-delivery-add/ot-dr-warehouse-delivery-add.component';

import { StockReceivingRegistrationComponent } from '@s_stock_rec/registration/stock-receiving-registration.component';

import { InventoryListComponent } from '@s_im/list-search/list/inventory-list.component';
import { InventoryDetailsComponent } from '@s_im/list-search/details/inventory-details.component';
import { InventoryStockHistoryComponent } from '@s_im/list-search/stock-history/inventory-stock-history.component';

// VALIDATORS


@NgModule({
  declarations: [       
    StoreBaseComponent,
    DashboardComponent,
    ReceiveFromTransferListComponent,
    ReceiveFromTransferUpdateComponent,
    OutgoingListComponent,
    OutgoingListDetailsComponent,
    RequestRegistrationComponent,
    SalesReleasingRegistrationComponent,
    ReturnRegistrationComponent,
    ReturnBreakageListComponent,
    ReturnBreakageDetailsComponent,
    ReleasedRegistrationComponent,
    StockRequestRegistrationComponent,
    ReceiveFromIncomingListComponent,
    ReceiveFromIncomingDetailsComponent,
    ReleasedListComponent,
    ReleasedListDetailsComponent,
    ReleasedListAdvancedSearchComponent,
    DeliveryRequestListComponent,
    DeliveryRequestStoreDeliveryAddComponent,
    DeliveryRequestUpdateStoreComponent,
    DeliveryRequestWarehouseDeliveryAddComponent,
    DeliveryRequestUpdateWarehouseComponent,
    OrderTransferListComponent,
    OrderTransferDetailsComponent,
    OrderTransferAdvancedSearchComponent,
    OrderTransferDeliveryRequestListComponent,
    OrderTransferDeliveryRequestUpdateStoreComponent,
    OrderTransferDeliveryRequestUpdateWarehouseComponent,
    OrderTransferDeliveryRequestWADComponent,
    DeliveryRequestUpdateWarehouseComponent,
    ReceiveFromIncomingDetailsComponent,
    ReturnBreakageDetailsComponent,
    RequestListComponent,
    RequestAdvancedSearchComponent,
    RequestDetailsComponent,
    RequestUpdateComponent,
    RequestStoreDeliveryAddComponent,
    RequestWarehouseDeliveryAddComponent,
    StockReceivingRegistrationComponent,
    InventoryListComponent,
    InventoryDetailsComponent,
    InventoryStockHistoryComponent

  ],
  imports: [
  	CommonModule,
    FormsModule, 
    HttpModule,
    ReactiveFormsModule,
    StoreRoutingModule,
    RouterModule,
    SharedModule
  ],
  providers: [
    BaseService,
    CommonViewService,
    PagerService,
    ItemService,
    ProductService,
    StockService,
    WarehouseService, 
    UserService,
    OrderRequestService,
    StoreService,
    StoreInventoriesService,
    StoreRequestService,
    WarehouseInventoriesService

  ]
})
export class StoreModule { }