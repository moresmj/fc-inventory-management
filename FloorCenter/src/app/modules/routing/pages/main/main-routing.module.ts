import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainBaseComponent }  from '@baseComponents/main/main-base.component';
import { DashboardComponent } from '@main/Dashboard/dashboard.component';
import { UserProfileComponent } from '@main/User-Profile/user-profile.component';
import { StockReceivingListComponent } from '@m_srm/list-search/list/sr-list.component';
import { InventoryListComponent } from '@m_im/list-search/inventory-list/inventory-list.component';
import { ProductListComponent } from '@m_pm/product-list/list/product-list.component';
import { ProductDetailComponent } from '@m_pm/product-list/details/product-detail.component';
import { UserListComponent } from '@m_um/user-list/user-list.component';
import { StoreListComponent } from '@m_settings/store-list/list/s-list.component';
import { WarehouseListComponent } from '@m_settings/warehouse-list/list/w-list.component';
import { OrderRequestListComponent } from '@m_rm/order-request/list/order-request-list.component';

const mainRoutes: Routes = [
	{ 
	  path: '',
      component: MainBaseComponent,
      children: [
      	{ path: 'dashboard', component: DashboardComponent },
      	{ path: 'profile', component: UserProfileComponent },
        { path: 'stock_list', component: StockReceivingListComponent },
        { path: 'inventory_list', component: InventoryListComponent },
        { path: 'product_list', component: ProductListComponent },
        { path: 'product_detail', component: ProductDetailComponent },
      	{ path: 'user_management', component: UserListComponent },
        { path: 'store_list', component: StoreListComponent },
        { path: 'warehouse_list', component: WarehouseListComponent },
        { path: 'order_request_list', component: OrderRequestListComponent }
      ]
	}
];

@NgModule({
  imports: [ RouterModule.forChild(mainRoutes) ],
  exports: [ RouterModule ]
})
export class MainRoutingModule{ }
