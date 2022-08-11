import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@library/_guards/auth.guard';

import { WarehouseBaseComponent }  from '@baseComponents/warehouse/warehouse-base.component';

import { DashboardComponent } from '@warehouse/dashboard/dashboard.component';
import { ReceiveItemListComponent } from '@warehouse/transaction/receive-items/list/receive-item-list.component';
import { ReceiveItemsFromOrdersRegistrationComponent } from '@warehouse/transaction/receive-items/from-orders/ri-fo-registration.component';
import { ReleaseItemListComponent } from '@warehouse/transaction/release-items/list/release-item-list.component';
import { InventoryListComponent } from '@w_im/list-search/list/im-list.component';
import { ReceiveReturnItemListComponent } from '@warehouse/transaction/receive-items/from-returns/list/receive-return-item-list.component';
import { ReceiveReturnDetailsComponent } from '@warehouse/transaction/receive-items/from-returns/_receive/ri-fr-details.component';

import { WarehouseStockHistoryComponent } from '@w_im/list-search/_stock-history/w-stock-history.component';
import { ImportPhysicalCountComponent } from '@w_im/import-physical-count/import-physical-count.component';
import { RegisterPhysicalCountComponent } from '@w_im/register-physical-count/register-physical-count.component';
import { RegisterBreakageComponent } from '@w_im/register-breakage/register-breakage.component';

import { PhysicalCountSummaryListComponent } from '@w_im/physical-count-summary/list/pcs-im-list.component';
import { PhysicalCountSummaryStockHistoryComponent } from '@w_im/physical-count-summary/_stock-history/stock-history.component';
import { AddItemWarehouseComponent } from '@warehouse/transaction/item/add-item.component';


import { WarehouseReleasingDetailsComponent } from '@w_im/list-search/releasing-details/w-releasing-details.component';

import { ChangeTonalityListComponent } from '@warehouse/transaction/change-tonality/list/change-tonality-list.component';

import { AdvanceOrderListComponent } from '@w_transaction/advance-order/list/advance-orders-list.component';

import { AllocateAdvanceOrderListComponent } from '@w_transaction/allocate-advance-order/list/allocate-advance-orders-list.component';
import { AllocateAdvanceOrderDetailsComponent } from '@w_transaction/allocate-advance-order/details/allocate-advance-orders-details.component';
import { AdjustReservedCountComponent } from '@components/warehouse/inventory-management/adjust-reserved-count/adjust-reserved-count.component';

const warehouseRoutes: Routes = [
	{ 
	    path: '',
      component: WarehouseBaseComponent,
      canActivate: [AuthGuard],
      children: [
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      	{ path: 'dashboard', component: DashboardComponent },
        { path: 'receive_list', component: ReceiveItemListComponent },//stock_list.html
        { path: 'receive_return_list', component: ReceiveReturnItemListComponent },//stock_list.html
        { path : 'receive_list/ri_fo_registration' , component: ReceiveItemsFromOrdersRegistrationComponent },//stock_registration.html
        { path: 'release_list', component : ReleaseItemListComponent },
        { path: 'inventory_list', component : InventoryListComponent },
        { path: 'receive_return_list/receive_return_details/:id', component : ReceiveReturnDetailsComponent },
        { path: 'inventory_list/item-history/:id', component: WarehouseStockHistoryComponent },
        { path: 'import-physical-count', component: ImportPhysicalCountComponent },
        { path: 'register-physical-count', component: RegisterPhysicalCountComponent },
        { path: 'register-breakage', component: RegisterBreakageComponent },
        { path: 'pc_summary_list', component : PhysicalCountSummaryListComponent },
        { path: 'pc_summary/item-history/:id', component: PhysicalCountSummaryStockHistoryComponent },
        { path: 'add_item', component: AddItemWarehouseComponent },
        { path: 'w-releasing-details/:id', component: WarehouseReleasingDetailsComponent },
        { path: 'change_tonality_list', component: ChangeTonalityListComponent },
        { path: 'advance_order_list', component: AdvanceOrderListComponent },
        { path: 'allocate_advance_order_list', component: AllocateAdvanceOrderListComponent },
        { path: 'allocate_advance_order_list/details/:id', component: AllocateAdvanceOrderDetailsComponent },
        { path: 'adjust-reserved-count', component: AdjustReservedCountComponent },

        

      ]
	}
];

@NgModule({
  imports: [ RouterModule.forChild(warehouseRoutes) ],
  exports: [ RouterModule ]
})
export class WarehouseRoutingModule{ }
