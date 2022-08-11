// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MainRoutingModule } from '@pageRoutes/main/main-routing.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@modules/pages/shared/shared.module';

// SERVICES
import { BaseService } from '@services/base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { PagerService } from '@services/common/pager.service';
import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { UserService } from '@services/user/user.service';
import { ProductService } from '@services/product/product.service';
import { OrderRequestService } from '@services/order_request/order-request.service';
import { StockService } from '@services/stock/stock.service';

import { WarehouseInventoriesService } from '@services/warehouse/warehouse-inventories.service';
import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreRequestService } from '@services/store-request/store-request.service';




// FILTERS
import { StoreListFilter } from '@filters/store/store-list.filter';
import { WarehouseListFilter } from '@filters/warehouse/warehouse-list.filter';
import { UserListFilter } from '@filters/user/user-list.filter';
import { ProductListFilter } from '@filters/product/product-list.filter';

// COMPONENTS
import { MainBaseComponent }  from '@baseComponents/main/main-base.component';
import { DashboardComponent } from '@main/Dashboard/dashboard.component';
import { UserProfileComponent } from '@main/User-Profile/user-profile.component';
import { StockReceivingListComponent } from '@m_srm/list-search/list/sr-list.component';
import { StockReceivingListAdvanceSearchComponent } from '@m_srm/list-search/advance-search/sr-list-advance-search.component';
import { StockReceivingListDetailsComponent } from '@m_srm/list-search/details/sr-list-details.component';
import { InventoryListComponent } from '@m_im/list-search/inventory-list/inventory-list.component';
import { InventoryListDetailsComponent } from '@m_im/list-search/inventory-details/inventory-list-details.component';
import { InventoryHistoryComponent } from '@m_im/list-search/inventory-history/inventory-history.component';
import { ProductListComponent } from '@m_pm/product-list/list/product-list.component';
import { ProductDetailComponent } from '@m_pm/product-list/details/product-detail.component';
import { ProductAddComponent } from '@m_pm/product-list/add/product-add.component';
import { UserListComponent } from '@m_um/user-list/user-list.component';
import { UserAddComponent } from '@m_um/user-add/user-add.component';
import { UserUpdateComponent } from '@m_um/user-update/user-update.component';
import { StoreListComponent } from '@m_settings/store-list/list/s-list.component';
import { StoreListAddComponent } from '@m_settings/store-list/add/s-list-add.component';
import { StoreListDetailsComponent } from '@m_settings/store-list/details/s-list-details.component';
import { WarehouseListComponent } from '@m_settings/warehouse-list/list/w-list.component';
import { WarehouseListAddComponent } from '@m_settings/warehouse-list/add/w-list-add.component';
import { WarehouseListDetailsComponent } from '@m_settings/warehouse-list/details/w-list-details.component';
import { OrderRequestListComponent } from '@m_rm/order-request/list/order-request-list.component';
import { OrderRequestListAdvanceSearchComponent } from '@m_rm/order-request/advance-search/orl-advance-search.component';
import { OrderRequestListDetailsComponent } from '@m_rm/order-request/details/orl-details.component';  


// VALIDATORS
import { CustomValidator } from '@validators/custom.validator';

@NgModule({
  declarations: [       
    MainBaseComponent,
    DashboardComponent,
    UserProfileComponent,
    StockReceivingListComponent,
    StockReceivingListAdvanceSearchComponent,
    StockReceivingListDetailsComponent,
    InventoryListComponent,
    InventoryListDetailsComponent,
    InventoryHistoryComponent,
    ProductListComponent,
    ProductAddComponent,
    ProductDetailComponent,
    UserListComponent,
    UserAddComponent,
    UserUpdateComponent,
    StoreListComponent,
    StoreListAddComponent,
    StoreListDetailsComponent,
    WarehouseListComponent,
    WarehouseListAddComponent,
    WarehouseListDetailsComponent,
    WarehouseListFilter,
    StoreListFilter,
    UserListFilter,
    ProductListFilter,
    OrderRequestListComponent,
    OrderRequestListAdvanceSearchComponent,
    OrderRequestListDetailsComponent
  ],
  imports: [
    CommonModule,
    MainRoutingModule,
    RouterModule,
    HttpModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers: [
    BaseService, 
    CommonViewService, 
    PagerService, 
    StoreService, 
    WarehouseService,
    ProductService,
    UserService,
    OrderRequestService,
    StockService,
    WarehouseInventoriesService,
    StoreInventoriesService,
    StoreRequestService
  ]
})
export class MainModule { }