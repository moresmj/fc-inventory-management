import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@library/_guards/auth.guard';

import { StoreBaseComponent }  from '@baseComponents/store/store-base.component';
import { DashboardComponent } from '@store/dashboard/dashboard.component';
import { OrderShowroomStockComponent } from '@s_transaction/orders/_showroom-stock/o-showroom-stock.component';
import { OrderStockListComponent } from '@store/transaction/orders/list/order-stock-list.component';
import { OrderStockDeliveryRequestListComponent } from '@store/transaction/orders/_delivery/list/os-dr-list.component';
import { ReceiveItemsListComponent } from '@store/transaction/receive-items/list/receive-items-list.component';
import { ReceiveItemsDetailsComponent } from '@store/transaction/receive-items/_receive/receive-items-details.component';
import { OrderForClientComponent } from '@s_transaction/orders/_for-client/o-for-client.component';
import { OrderInterbranchComponent } from '@s_transaction/orders/_interbranch/o-interbranch.component';
import { ReleasingListComponent } from '@s_transaction/releasing/list/releasing-list.component';
import { ReleaseSalesRegistrationComponent } from '@s_transaction/releasing/_release-item/sales/r-sales-registration.component';
import { ReleaseInterbranchRegistrationComponent } from '@s_transaction/releasing/_release-item/interbranch/r-interbranch-registration.component';
import { InventoryListComponent } from '@s_im/list-search/list/im-list.component';
import { StoreReleasingDetailsComponent } from '@s_im/list-search/releasing-details/s-releasing-details.component';
import { StockHistoryComponent } from '@s_im/list-search/_stock-history/stock-history.component';

import { SalesOrderListComponent } from '@s_transaction/sales-orders/list/so-list.component';
import { SalesOrderNewComponent } from '@s_transaction/sales-orders/_new-sales-order/so-new-sales-order.component';
import { SalesOrderDeliveryListComponent } from '@s_transaction/sales-orders/_delivery/list/so-delivery-list.component';

import { BranchOrdersListComponent } from '@s_transaction/branch-order/list/bo-list.component';
import { ForClientReleasingListComponent } from '@s_transaction/releasing/for-client/list/fr-releasing-list.component';
import { ForClientReleasingUpdateComponent } from '@s_transaction/releasing/for-client/update/fr-releasing-update.component';
import { SalesOrderReleasingListComponent } from '@s_transaction/releasing/_sales-order/list/so-releasing-list.component';
import { SameDaySalesReleasingListComponent } from '@s_transaction/releasing/_same-day-sales/list/sds-releasing-list.component';
import { TransferReleasingListComponent } from '@s_transaction/releasing/_transfer/list/t-releasing-list.component';
import { ReturnDeliveryRequestListComponent } from '@s_transaction/returns/_delivery/list/re-dr-list.component';

import { ReturnListComponent } from '@s_transaction/returns/list/return-list.component';
import { RTVPurchaseReturnsComponent } from '@s_transaction/returns/_create-return-request/_rtv-purchase-returns/rtv-purchase-returns.component';
import { ClientReturnRegistration } from '@s_transaction/returns/_create-return-request/client-return/re-client-return.component';
import { ReturnBackloadListComponent } from '@s_transaction/returns/_create-return-request/backload/list/re-backload-list.component';
import { BreakageReturnsComponent } from '@s_transaction/returns/_create-return-request/breakage/breakage.component';

import { ReturnsReleasingListComponent } from '@s_transaction/releasing/_returns/list/re-releasing-list.component';

import { ClientReturnListComponent } from '@s_transaction/returns/_create-return-request/client-return/list/re-cr-list.component';
import { ClientReturnDetailsComponent } from '@s_transaction/returns/_create-return-request/client-return/_return/re-cr-details.component';

import { ReturnsReceiveItemsListComponent } from '@store/transaction/receive-items/receive-returns/list/returns-receive-items-list.component';
import { ReturnsReceiveItemsDetailsComponent } from '@store/transaction/receive-items/receive-returns/_receive/returns-receive-items-details.component';

import { ImportPhysicalCountListComponent } from '@s_im/import-physical-count/list/ip-count-list.component';
import { RegisterStorePhysicalCountComponent } from '@s_im/register-physical-count/register-store-physical-count.component';
import { RegisterStoreBreakageComponent } from '@s_im/register-breakage/register-store-breakage.component';

import { PhysicalCountSummaryListComponent } from '@s_im/physical-count-summary/list/pcs-im-list.component';
import { PhysicalCountSummaryStockHistoryComponent } from '@s_im/physical-count-summary/_stock-history/stock-history.component';

import { AdvanceOrderComponent } from '@s_transaction/orders/_advance-order/advance-order.component';

import { AdvanceOrderStoreComponent } from '@s_transaction/advance-order/advance-order.component';

const storeRoutes: Routes = [
	{ 
	    path: '',
      component: StoreBaseComponent,
      canActivate: [AuthGuard],
      children: [
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      	{ path: 'dashboard', component: DashboardComponent },
        { path: 'orders', component: OrderStockListComponent },
        { path: 'orders/showroom_stock', component: OrderShowroomStockComponent }, // showroom_stock.html
        { path: 'orders/stock_delivery_request/:id', component: OrderStockDeliveryRequestListComponent},
        { path: 'orders/interbranch', component : OrderInterbranchComponent },
        { path: 'receive-items', component: ReceiveItemsListComponent },
        { path: 'receive-items/details/:id', component: ReceiveItemsDetailsComponent },
        { path: 'orders/for_client', component: OrderForClientComponent },// client.html
        { path: 'releasing' , component: ReleasingListComponent },
        { path: 'same-day-sales-releasing/sales_registration', component : ReleaseSalesRegistrationComponent },
        { path: 'releasing/interbranch_registration', component : ReleaseInterbranchRegistrationComponent },
        { path: 'inventory_list', component : InventoryListComponent },

        { path: 's-releasing-details/:id', component: StoreReleasingDetailsComponent},
        { path: 'inventory/item-history/:id', component: StockHistoryComponent },
        { path: 'sales', component : SalesOrderListComponent },
        { path: 'sales/new_sales_order', component : SalesOrderNewComponent },
        { path: 'sales/order_details/:id', component : SalesOrderDeliveryListComponent },
        { path: 'branch-orders', component : BranchOrdersListComponent },
        { path: 'for-client-releasing', component : ForClientReleasingListComponent},
        { path: 'sales-order-releasing', component : SalesOrderReleasingListComponent},
        { path: 'same-day-sales-releasing', component : SameDaySalesReleasingListComponent},
        { path: 'returns-releasing', component : ReturnsReleasingListComponent},
        { path: 'transfer-releasing', component : TransferReleasingListComponent},
        { path: 'returns', component : ReturnListComponent },
        { path: 'returns/delivery_request/:id', component : ReturnDeliveryRequestListComponent },
        { path: 'returns/purchase_return', component: RTVPurchaseReturnsComponent },
        { path: 'returns/breakage', component: BreakageReturnsComponent },
       // { path: 'returns/client_return', component : ClientReturnRegistration },
        { path: 'returns/backload', component : ReturnBackloadListComponent },
        { path: 'returns/client_return_list', component : ClientReturnListComponent },
        { path: 'returns/client_return_list/client_return_detail/:id', component : ClientReturnDetailsComponent },
        { path: 'returns-receive-items', component: ReturnsReceiveItemsListComponent },
        { path: 'returns-receive-items/details/:id', component: ReturnsReceiveItemsDetailsComponent },
        { path: 'import-physical-count', component: ImportPhysicalCountListComponent },
        { path: 'register-store-physical-count', component: RegisterStorePhysicalCountComponent },
        { path: 'register-store-breakage', component: RegisterStoreBreakageComponent },
        { path: 'pc_summary_list', component : PhysicalCountSummaryListComponent },
        { path: 'pc_summary/item-history/:id', component: PhysicalCountSummaryStockHistoryComponent },
        { path: 'orders/advance_order', component: AdvanceOrderComponent },
        { path: 'advance_order_list', component: AdvanceOrderStoreComponent },

      ]
	}
];

@NgModule({
  imports: [ RouterModule.forChild(storeRoutes) ],
  exports: [ RouterModule ]
})
export class StoreRoutingModule{ }
