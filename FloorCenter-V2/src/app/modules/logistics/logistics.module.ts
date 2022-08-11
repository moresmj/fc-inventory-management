// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { LogisticsRoutingModule } from '@modules/logistics/logistics-routing.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@modules/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AuthGuard } from '@library/_guards/auth.guard';

// FILTERS

// VALIDATORS

// SERVICES
import { PageModuleService } from '@services/common/pageModule.service';
import { BaseService } from '@services/base.service';
import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';
import { PagerService } from '@services/common/pager.service';
import { UserService } from '@services/user/user.service';
import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { ItemService } from '@services/item/item.service';
import { ApproveRequestsService } from '@services/approve-requests/approve-requests.service';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { OrderService } from '@services/order/order.service';
import { DeliveriesService } from '@services/deliveries/deliveries.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { ReleasingService } from '@services/releasing/releasing.service';
import { InventoriesService } from '@services/inventories/inventories.service';
import { SalesOrderService } from '@services/sales-order/sales-order.service';
import { BranchOrderService } from '@services/branch-order/branch-order.service';
import { ClientReleasingService } from '@services/releasing/client-releasing.service';
import { SalesOrderReleasingService } from '@services/releasing/sales-order-releasing.service';
import { SameDaySalesReleasingService } from '@services/releasing/same-day-sales-releasing.service';
import { TransferReleasingService } from '@services/releasing/transfer-releasing.service';
import { ApiBaseService } from '@services/api-base.service';

// COMPONENTS
import { LogisticsBaseComponent }  from '@baseComponents/logistics/logistics-base.component';
import { DashboardComponent } from '@logistics/dashboard/dashboard.component';

import { OrdersDeliveryListComponent } from '@l_deliveries/orders/list/o-delivery-list.component';
import { OrdersDeliveryUpdateComponent } from '@l_deliveries/orders/update/o-delivery-update.component';
import { OrdersDeliveryAdvancedSearchComponent } from '@l_deliveries/orders/advanced-search/o-delivery-advanced-search.component';
import { SalesDeliveryListComponent } from '@l_deliveries/sales/list/s-delivery-list.component';
import { SalesDeliveryUpdateComponent } from '@l_deliveries/sales/update/s-delivery-update.component';
import { SalesDeliveryAdvancedSearchComponent } from '@l_deliveries/sales/advanced-search/s-delivery-advanced-search.component';

import { ReturnDeliveryListComponent } from '@logistics/return-deliveries/list/rd-list.component';
import { ReturnDeliveryUpdateComponent } from '@logistics/return-deliveries/update/rd-update.component';
import { ReturnDeliveryAdvancedSearchComponent } from '@logistics/return-deliveries/advanced-search/rd-advanced-search.component';


import { ReturnClientDeliveryListComponent } from '@logistics/return-client/list/rc-list.component';
import { ReturnClientDeliveryUpdateComponent } from '@logistics/return-client/update/rc-update.component';
import { ReturnClientDeliveryAdvancedSearchComponent } from '@logistics/return-client/advanced-search/rc-advanced-search.component';

@NgModule({
	imports : [
		CommonModule,
		HttpModule,
		LogisticsRoutingModule,
		RouterModule,
		SharedModule,
		FormsModule,
		ReactiveFormsModule
	],
	declarations : [
		LogisticsBaseComponent,
		DashboardComponent,
		OrdersDeliveryListComponent,
		OrdersDeliveryUpdateComponent,
		OrdersDeliveryAdvancedSearchComponent,
		SalesDeliveryListComponent,
		SalesDeliveryUpdateComponent,
		SalesDeliveryAdvancedSearchComponent,
		ReturnDeliveryListComponent,
		ReturnDeliveryUpdateComponent,
		ReturnDeliveryAdvancedSearchComponent,
		ReturnClientDeliveryListComponent,
		ReturnClientDeliveryUpdateComponent,
		ReturnClientDeliveryAdvancedSearchComponent
	],
	providers : [
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
		OrderService,
		DeliveriesService,
		OrderStockService,
		ReleaseItemsService,
		ReleasingService,
		InventoriesService,
		SalesOrderService,
		BranchOrderService,
		ClientReleasingService,
		SalesOrderReleasingService,
		SameDaySalesReleasingService,
		TransferReleasingService,
		ApiBaseService
	]
})

export class LogisticsModule {}