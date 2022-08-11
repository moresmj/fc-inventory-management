// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { StoreRoutingModule } from '@modules/store/store-routing.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@modules/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';


// GUARDS
import { AuthGuard } from '@library/_guards/auth.guard';

//import module to reuse component
import { WarehouseModule } from '@modules/warehouse/warehouse.module';

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
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { DeliveriesService } from '@services/deliveries/deliveries.service';
import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { ReleasingService } from '@services/releasing/releasing.service';
import { InventoriesService } from '@services/inventories/inventories.service';
import { SalesOrderService } from '@services/sales-order/sales-order.service';
import { TransferService } from '@services/transfer/transfer.service';
import { BranchOrderService } from '@services/branch-order/branch-order.service';
import { ClientReleasingService } from '@services/releasing/client-releasing.service';
import { SalesOrderReleasingService } from '@services/releasing/sales-order-releasing.service';
import { SameDaySalesReleasingService } from '@services/releasing/same-day-sales-releasing.service';
import { TransferReleasingService } from '@services/releasing/transfer-releasing.service';
import { ApiBaseService } from '@services/api-base.service';

// COMPONENTS
import { StoreBaseComponent }  from '@baseComponents/store/store-base.component';
import { DashboardComponent } from '@store/dashboard/dashboard.component';
import { OrderShowroomStockComponent } from '@s_transaction/orders/_showroom-stock/o-showroom-stock.component';
import { OrderStockListComponent } from '@store/transaction/orders/list/order-stock-list.component';
import { OrderStockAdvanceSearchComponent} from '@store/transaction/orders/advance-search/order-stock-advance-search.component';
import { OrderStockDetailsComponent } from '@store/transaction/orders/details/order-stock-details.component';
import { OrderStockDeliveryRequestListComponent } from '@store/transaction/orders/_delivery/list/os-dr-list.component';
import { OrderStockDeliveryRequestShowroomAddComponent } from '@store/transaction/orders/_delivery/add/showroom/os-dr-sr-add.component';
import { OrderStockDeliveryRequestDetailsComponent } from '@store/transaction/orders/_delivery/details/os-dr-details.component';
import { ReceiveItemsListComponent } from '@store/transaction/receive-items/list/receive-items-list.component';
import { ReceiveItemsAdvancedSearchComponent } from '@store/transaction/receive-items/advanced-search/receive-items-advanced-search.component';
import { ReceiveItemsDetailsComponent } from '@store/transaction/receive-items/_receive/receive-items-details.component';
import { OrderForClientComponent } from '@s_transaction/orders/_for-client/o-for-client.component';
import { OrderInterbranchComponent } from '@s_transaction/orders/_interbranch/o-interbranch.component';
import { ReleasingListComponent } from '@s_transaction/releasing/list/releasing-list.component';
import { ReleasingAdvancedSearchComponent } from '@s_transaction/releasing/advanced-search/releasing-advanced-search.component';
import { ReleasingUpdateComponent } from '@s_transaction/releasing/update/releasing-update.component';
import { ReleaseSalesRegistrationComponent } from '@s_transaction/releasing/_release-item/sales/r-sales-registration.component';
import { ReleaseInterbranchRegistrationComponent } from '@s_transaction/releasing/_release-item/interbranch/r-interbranch-registration.component';
import { OrderStockDeliveryRequestClientAddComponent}  from '@store/transaction/orders/_delivery/add/client/os-dr-cr-add.component';

import { InventoryListComponent } from '@s_im/list-search/list/im-list.component';
import { StoreReleasingDetailsComponent } from '@s_im/list-search/releasing-details/s-releasing-details.component';

import { InventoryAdvancedSearchComponent } from '@s_im/list-search/advanced-search/im-advanced-search.component';
import { InventoryDetailsComponent } from '@s_im/list-search/details/im-details.component';
import { StockHistoryComponent } from '@s_im/list-search/_stock-history/stock-history.component';
import { SalesOrderListComponent } from '@s_transaction/sales-orders/list/so-list.component';
import { SalesOrderAdvancedSearchComponent } from '@s_transaction/sales-orders/advanced-search/so-advanced-search.component';
import { SalesOrderNewComponent } from '@s_transaction/sales-orders/_new-sales-order/so-new-sales-order.component';
import { SalesOrderDeliveryListComponent } from '@s_transaction/sales-orders/_delivery/list/so-delivery-list.component';
import { SalesOrderDeliveryDetailsComponent } from '@s_transaction/sales-orders/_delivery/details/so-delivery-details.component';
import { SalesOrderDeliveryAddClientComponent } from '@s_transaction/sales-orders/_delivery/add-client-delivery/so-delivery-add-client.component';
import { BranchOrdersListComponent } from '@s_transaction/branch-order/list/bo-list.component';
import { BranchOrdersUpdateComponent } from '@s_transaction/branch-order/update/bo-update.component';
import { BranchOrdersAdvancedSearchComponent } from '@s_transaction/branch-order/advanced-search/bo-advanced-search.component';
import { ForClientReleasingListComponent } from '@s_transaction/releasing/for-client/list/fr-releasing-list.component';
import { ForClientReleasingUpdateComponent } from '@s_transaction/releasing/for-client/update/fr-releasing-update.component';
import { ForClientReleasingAdvancedSearchComponent } from '@s_transaction/releasing/for-client/advanced-search/fr-releasing-advanced-search.component';

import { SalesOrderReleasingListComponent } from '@s_transaction/releasing/_sales-order/list/so-releasing-list.component';
import { SalesOrderReleasingUpdateComponent } from '@s_transaction/releasing/_sales-order/update/so-releasing-update.component';
import { SalesOrderReleasingAdvancedSearchComponent } from '@s_transaction/releasing/_sales-order/advanced-search/so-releasing-advanced-search.component';

import { SameDaySalesReleasingListComponent } from '@s_transaction/releasing/_same-day-sales/list/sds-releasing-list.component';
import { SameDaySalesReleasingUpdateComponent } from '@s_transaction/releasing/_same-day-sales/update/sds-releasing-update.component';
import { SameDaySalesReleasingAdvancedSearchComponent } from '@s_transaction/releasing/_same-day-sales/advanced-search/sds-releasing-advanced-search.component';

import { TransferReleasingListComponent } from '@s_transaction/releasing/_transfer/list/t-releasing-list.component';
import { TransferReleasingUpdateComponent } from '@s_transaction/releasing/_transfer/update/t-releasing-update.component';
import { TransferReleasingAdvancedSearchComponent } from '@s_transaction/releasing/_transfer/advanced-search/t-releasing-advanced-search.component';

import { ReturnAdvancedSearchComponent } from '@s_transaction/returns/advanced-search/return-advanced-search.component';
import { ReturnListComponent } from '@s_transaction/returns/list/return-list.component';
import { ReturnDetailsComponent } from '@s_transaction/returns/details/return-details.component';
import { ReturnDeliveryRequestListComponent } from '@s_transaction/returns/_delivery/list/re-dr-list.component';
import { ReturnDeliveryRequestDetailsComponent } from '@s_transaction/returns/_delivery/details/re-dr-details.component';
import { ReturnDeliveryRequestAddComponent } from '@s_transaction/returns/_delivery/add/re-dr-add.component';
import { RTVPurchaseReturnsComponent } from '@s_transaction/returns/_create-return-request/_rtv-purchase-returns/rtv-purchase-returns.component';
import { ClientReturnRegistration } from '@s_transaction/returns/_create-return-request/client-return/re-client-return.component';
import { ReturnBackloadListComponent } from '@s_transaction/returns/_create-return-request/backload/list/re-backload-list.component';
import { ReturnBackloadUpdateComponent } from '@s_transaction/returns/_create-return-request/backload/update/re-backload-update.component';
import { ReturnBackloadAdvancedSearchComponent } from '@s_transaction/returns/_create-return-request/backload/advanced-search/re-backload-advanced-search.component';
import { BreakageReturnsComponent } from '@s_transaction/returns/_create-return-request/breakage/breakage.component';

import { ReturnsReleasingListComponent } from '@s_transaction/releasing/_returns/list/re-releasing-list.component';
import { ReturnsReleasingUpdateComponent } from '@s_transaction/releasing/_returns/update/re-releasing-update.component';
import { ReturnsReleasingAdvancedSearchComponent } from '@s_transaction/releasing/_returns/advanced-search/re-releasing-advanced-search.component';

import { ClientReturnListComponent } from '@s_transaction/returns/_create-return-request/client-return/list/re-cr-list.component';
import { ClientReturnDetailsComponent } from '@s_transaction/returns/_create-return-request/client-return/_return/re-cr-details.component';


import { ReturnsReceiveItemsListComponent } from '@store/transaction/receive-items/receive-returns/list/returns-receive-items-list.component';
import { ReturnsReceiveItemsAdvancedSearchComponent } from '@store/transaction/receive-items/receive-returns/advanced-search/returns-receive-items-advanced-search.component';
import { ReturnsReceiveItemsDetailsComponent } from '@store/transaction/receive-items/receive-returns/_receive/returns-receive-items-details.component';

import { ImportPhysicalCountListComponent } from '@s_im/import-physical-count/list/ip-count-list.component';
import { RegisterStorePhysicalCountComponent } from '@s_im/register-physical-count/register-store-physical-count.component';
import { RegisterStoreBreakageComponent } from '@s_im/register-breakage/register-store-breakage.component';

import { PhysicalCountSummaryListComponent } from '@s_im/physical-count-summary/list/pcs-im-list.component';
import { PhysicalCountSummaryStockHistoryComponent } from '@s_im/physical-count-summary/_stock-history/stock-history.component';
import { PhysicalCountSummaryDetailsComponent } from '@s_im/physical-count-summary/details/pcs-im-details.component';
import { AddClientSIComponent } from '@store/transaction/orders/add-client-si/add-client-si.component';
import { AdvanceOrderComponent } from '@s_transaction/orders/_advance-order/advance-order.component';

import { AdvanceOrderStoreComponent } from '@s_transaction/advance-order/advance-order.component';

@NgModule({
	imports : [
		CommonModule,
		StoreRoutingModule,
		RouterModule,
		HttpModule,
		FormsModule,
		ReactiveFormsModule,
		SharedModule,
		FormsModule,
		ReactiveFormsModule,
		WarehouseModule,
		Ng4LoadingSpinnerModule.forRoot()

	],
	declarations : [
		StoreBaseComponent,
		DashboardComponent,
		OrderShowroomStockComponent,
		OrderStockListComponent,
		OrderStockAdvanceSearchComponent,
		OrderStockDetailsComponent,
		OrderStockDeliveryRequestListComponent,
		OrderStockDeliveryRequestShowroomAddComponent,
		OrderStockDeliveryRequestDetailsComponent,
		ReceiveItemsListComponent,
		ReceiveItemsAdvancedSearchComponent,
		ReceiveItemsDetailsComponent,
		OrderForClientComponent,
		OrderInterbranchComponent,
		ReleasingListComponent,
		ReleasingAdvancedSearchComponent,
		ReleasingUpdateComponent,
		ReleaseSalesRegistrationComponent,
		OrderStockDeliveryRequestClientAddComponent,
		InventoryListComponent,
		InventoryDetailsComponent,
		StoreReleasingDetailsComponent,
		InventoryAdvancedSearchComponent,
		StockHistoryComponent,
		ReleaseInterbranchRegistrationComponent,
		SalesOrderListComponent,
		SalesOrderAdvancedSearchComponent,
		SalesOrderNewComponent,
		SalesOrderDeliveryListComponent,
		SalesOrderDeliveryDetailsComponent,
		SalesOrderDeliveryAddClientComponent,
		BranchOrdersListComponent,
		BranchOrdersUpdateComponent,
		BranchOrdersAdvancedSearchComponent,
		ForClientReleasingListComponent,
		ForClientReleasingUpdateComponent,
		ForClientReleasingAdvancedSearchComponent,
		SalesOrderReleasingListComponent,
		SalesOrderReleasingUpdateComponent,
		SalesOrderReleasingAdvancedSearchComponent,
		SameDaySalesReleasingListComponent,
		SameDaySalesReleasingUpdateComponent,
		SameDaySalesReleasingAdvancedSearchComponent,
		TransferReleasingListComponent,
		TransferReleasingUpdateComponent,
		TransferReleasingAdvancedSearchComponent,
		ReturnListComponent,
		ReturnDetailsComponent,
		ReturnDeliveryRequestListComponent,
		ReturnDeliveryRequestDetailsComponent,
		ReturnDeliveryRequestAddComponent,
		RTVPurchaseReturnsComponent,
		ClientReturnRegistration,
    	BreakageReturnsComponent,
		ReturnBackloadListComponent,
		ReturnBackloadUpdateComponent,
		ReturnBackloadAdvancedSearchComponent,
		ReturnsReleasingListComponent,
		ReturnsReleasingUpdateComponent,
		ReturnsReleasingAdvancedSearchComponent,
		ClientReturnListComponent,
		ClientReturnDetailsComponent,
		ReturnsReceiveItemsListComponent,
		ReturnsReceiveItemsAdvancedSearchComponent,
		ReturnsReceiveItemsDetailsComponent,
		ImportPhysicalCountListComponent,
    	RegisterStorePhysicalCountComponent,
    	RegisterStoreBreakageComponent,
	  	PhysicalCountSummaryListComponent,
		PhysicalCountSummaryStockHistoryComponent,
		PhysicalCountSummaryDetailsComponent,
		AddClientSIComponent,
		ReturnAdvancedSearchComponent,
		AdvanceOrderComponent,
		AdvanceOrderStoreComponent
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
		OrderStockService,
		DeliveriesService,
		ReleaseItemsService,
		ReleasingService,
		InventoriesService,
		SalesOrderService,
		TransferService,
		BranchOrderService,
		ClientReleasingService,
		SalesOrderReleasingService,
		SameDaySalesReleasingService,
		TransferReleasingService,
		ApiBaseService
	]
})

export class StoreModule {}
