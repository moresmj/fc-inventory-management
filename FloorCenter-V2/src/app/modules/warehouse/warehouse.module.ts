// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { WarehouseRoutingModule } from '@modules/warehouse/warehouse-routing.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@modules/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AuthGuard } from '@library/_guards/auth.guard';
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

import { MainModule } from '@modules/main/main.module';
// FILTERS

// VALIDATORS

// SERVICES
import { PageModuleService } from '@services/common/pageModule.service';
import { BaseService } from '@services/base.service';
import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { PagerService } from '@services/common/pager.service';
import { ItemService } from '@services/item/item.service';
import { UserService } from '@services/user/user.service';
import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { ApproveRequestsService } from '@services/approve-requests/approve-requests.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderService } from '@services/order/order.service';
import { DeliveriesService } from '@services/deliveries/deliveries.service';
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
import { WarehouseBaseComponent }  from '@baseComponents/warehouse/warehouse-base.component';
import { DashboardComponent } from '@warehouse/dashboard/dashboard.component';
import { ReceiveItemListComponent } from '@warehouse/transaction/receive-items/list/receive-item-list.component';
import { ReceiveItemDetailsComponent } from '@warehouse/transaction/receive-items/details/receive-item-details.component';
import { ReceiveItemAdvanceSearchComponent } from '@warehouse/transaction/receive-items/advance-search/receive-item-advance-search.component';
import { ReceiveItemsFromOrdersRegistrationComponent } from '@warehouse/transaction/receive-items/from-orders/ri-fo-registration.component';
import { ReleaseItemListComponent } from '@warehouse/transaction/release-items/list/release-item-list.component';
import { ReleaseItemDetailsComponent } from '@warehouse/transaction/release-items/release/release-item-details.component';
import { ReleaseItemAdvanceSearchComponent } from '@warehouse/transaction/release-items/advance-search/release-item-advance-search.component';
import { InventoryListComponent } from '@w_im/list-search/list/im-list.component';
import { InventoryAdvancedSearchComponent } from '@w_im/list-search/advanced-search/im-advanced-search.component';
import { InventoryDetailsComponent } from '@w_im/list-search/details/im-details.component';
import { WarehouseStockHistoryComponent } from '@w_im/list-search/_stock-history/w-stock-history.component';

import { ReceiveReturnItemListComponent } from '@warehouse/transaction/receive-items/from-returns/list/receive-return-item-list.component';
import { ReceiveReturnDetailsComponent } from '@warehouse/transaction/receive-items/from-returns/_receive/ri-fr-details.component';
import { ReceiveReturnItemsAdvancedSearchComponent } from '@warehouse/transaction/receive-items/from-returns/advanced-search/receive-return-items-advanced-search.component';

import { ImportPhysicalCountComponent } from '@w_im/import-physical-count/import-physical-count.component';
import { RegisterPhysicalCountComponent } from '@w_im/register-physical-count/register-physical-count.component';
import { RegisterBreakageComponent } from '@w_im/register-breakage/register-breakage.component';


import { ItemAddComponent } from '@warehouse/transaction/receive-items/from-orders/add-item/item-add.component';

import { AddSizeComponent } from '@warehouse/transaction/receive-items/from-orders/add-item/add-size/add-size.component';


import { PhysicalCountSummaryListComponent } from '@w_im/physical-count-summary/list/pcs-im-list.component';
import { PhysicalCountSummaryStockHistoryComponent } from '@w_im/physical-count-summary/_stock-history/stock-history.component';
import { PhysicalCountSummaryDetailsComponent } from '@w_im/physical-count-summary/details/pcs-im-details.component';


import { AddItemWarehouseComponent } from '@warehouse/transaction/item/add-item.component';


import { WarehouseReleasingDetailsComponent } from '@w_im/list-search/releasing-details/w-releasing-details.component';

import { ChangeTonalityListComponent } from '@warehouse/transaction/change-tonality/list/change-tonality-list.component';
import { ChangeTonalityAdvanceSearchComponent } from '@warehouse/transaction/change-tonality/advance-search/change-tonality-advance-search.component';
import { ChangeTonalityDetailsComponent } from '@warehouse/transaction/change-tonality/details/change-tonality-details.component';

import { AdvanceOrderListComponent } from '@w_transaction/advance-order/list/advance-orders-list.component';
import { AdvanceOrderAdvancedSearchComponent } from '@w_transaction/advance-order/advanced-search/advance-orders-advanced-search.component';
import { AdvanceOrderDetailsComponent } from '@w_transaction/advance-order/details/advance-orders-details.component';

import { AllocateAdvanceOrderListComponent } from '@w_transaction/allocate-advance-order/list/allocate-advance-orders-list.component';
import { AllocateAdvanceOrderAdvancedSearchComponent } from '@w_transaction/allocate-advance-order/advanced-search/allocate-advance-orders-advanced-search.component';
import { AllocateAdvanceOrderDetailsComponent } from '@w_transaction/allocate-advance-order/details//allocate-advance-orders-details.component';
import { AdjustReservedCountComponent } from '@components/warehouse/inventory-management/adjust-reserved-count/adjust-reserved-count.component';

@NgModule({
	imports : [
		CommonModule,
		HttpModule,
		WarehouseRoutingModule,
		RouterModule,
		SharedModule,
		FormsModule,
	    ReactiveFormsModule,
		Ng4LoadingSpinnerModule.forRoot(),
		MainModule
	],
	declarations : [
		AddItemWarehouseComponent,
		WarehouseBaseComponent,
		DashboardComponent,
		ReceiveItemListComponent,
		ReceiveItemDetailsComponent,
		ReceiveItemAdvanceSearchComponent,
		ReceiveItemsFromOrdersRegistrationComponent,
		ReleaseItemListComponent,
		ReleaseItemDetailsComponent,
		ReleaseItemAdvanceSearchComponent,
		InventoryListComponent,
		InventoryAdvancedSearchComponent,
		InventoryDetailsComponent,
		ReceiveReturnItemListComponent,
		ReceiveReturnDetailsComponent,
		ReceiveReturnItemsAdvancedSearchComponent,
		WarehouseStockHistoryComponent,
	    ImportPhysicalCountComponent,
		RegisterPhysicalCountComponent,
		RegisterBreakageComponent,
		ItemAddComponent,
		AddSizeComponent,
		PhysicalCountSummaryListComponent,
		PhysicalCountSummaryStockHistoryComponent,
		PhysicalCountSummaryDetailsComponent,

		WarehouseReleasingDetailsComponent,

		ChangeTonalityListComponent,
		ChangeTonalityAdvanceSearchComponent,
		ChangeTonalityDetailsComponent,
		AdvanceOrderListComponent,
		AdvanceOrderAdvancedSearchComponent,
		AdvanceOrderDetailsComponent,

		AllocateAdvanceOrderListComponent,
		AllocateAdvanceOrderAdvancedSearchComponent,
		AllocateAdvanceOrderDetailsComponent,

		AdjustReservedCountComponent,

	],
	providers : [
		AuthGuard,
		PageModuleService,
		BaseService,
		RequestService,
		CommonViewService, 
		ReceiveItemsService,
		ItemService,
		UserService,
		StoreService,
		WarehouseService,
		PagerService,
		ApproveRequestsService,
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
	],

  	exports: [AdvanceOrderListComponent, AdvanceOrderAdvancedSearchComponent, AdvanceOrderDetailsComponent]
})

export class WarehouseModule {}
