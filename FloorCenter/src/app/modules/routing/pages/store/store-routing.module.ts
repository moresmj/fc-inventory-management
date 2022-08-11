import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { StoreBaseComponent }  from '@baseComponents/store/store-base.component';
import { DashboardComponent } from '@store/Dashboard/dashboard.component';
import { ReceiveFromTransferListComponent } from '@s_srm/receive-from-transfer/list/rft-list.component';
import { OutgoingListComponent } from '@s_stm/outgoing-list/list/og-list.component';
import { RequestRegistrationComponent } from '@s_stm/request-registration/rr.component';
import { SalesReleasingRegistrationComponent} from '@s_sar/registration/sl-registration.component';
import { ReturnRegistrationComponent } from '@s_rb/registration/return-registration.component';
import { ReturnBreakageListComponent } from '@s_rb/return-breakage/list/rb-list.component';
import { StockRequestRegistrationComponent } from '@s_stock_req/registration/sr-registration.component';
import { ReleasedRegistrationComponent } from '@s_sar/released-registration/rel-registration.component';
import { ReceiveFromIncomingListComponent } from '@s_stock_rec/receive-from-incoming/list/rfi-list.component';
import { ReceiveFromIncomingDetailsComponent } from '@s_stock_rec/receive-from-incoming/receive/details/rfi-details.component';
import { RequestListComponent } from '@s_drm/request-list/list/request-list.component';
import { RequestDetailsComponent } from '@s_drm/request-list/details/dr-details.component';
import { ReceiveListComponent } from '@s_stock_rec/receive-list/list/rl-list.component';

import { ReleasedListComponent } from '@s_sar/released-list/list/released-list.component';
import { DeliveryRequestListComponent } from '@s_sar/released-list/_delivery/list/delivery-request-list.component';

import { OrderTransferListComponent } from '@s_stock_req/order-transfer-list/list/order-transfer-list.component';
import { OrderTransferDeliveryRequestListComponent } from '@s_stock_req/order-transfer-list/_delivery/list/ot-delivery-request-list.component';

import { StockReceivingRegistrationComponent } from '@s_stock_rec/registration/stock-receiving-registration.component';
import { InventoryListComponent } from '@s_im/list-search/list/inventory-list.component';

const storeRoutes: Routes = [
	{ 
	  path: '',
      component: StoreBaseComponent,
      children: [
      	{ path: 'dashboard', component: DashboardComponent },
        { path: 'stock_receive', component: ReceiveFromTransferListComponent},  // MISSING
      	{ path: 'outgoing_list', component: OutgoingListComponent },   // MISSING
        { path: 'request_registration', component: RequestRegistrationComponent},
        { path: 'sales_registration', component: SalesReleasingRegistrationComponent},  // MISSING
        { path: 'return_registration', component: ReturnRegistrationComponent},
        { path: 'return_list', component: ReturnBreakageListComponent},
        { path: 'stock_registration', component: StockRequestRegistrationComponent},
        { path: 'releasing_registration', component: ReleasedRegistrationComponent},
        { path: 'receive_incoming', component : ReceiveFromIncomingListComponent },
        { path: 'receive_incoming/details/:transactionNumber', component : ReceiveFromIncomingDetailsComponent },      
        { path: 'return_list', component: ReturnBreakageListComponent},
        { path: 'request_list', component: RequestListComponent	},
        { path: 'request_list/delivery_request', component: RequestDetailsComponent},
        { path: 'receive_incoming', component : ReceiveFromIncomingListComponent },
        { path: 'receive_incoming/details/:transactionNumber', component : ReceiveFromIncomingDetailsComponent },
        { path: 'sales_list', component : ReleasedListComponent },                                                          //  sales_list.html
        { path: 'sales_list/delivery_request/:transactionNumber', component : DeliveryRequestListComponent },               //  delivery_request.html  
        { path: 'stock_list', component : OrderTransferListComponent },                                                     //  stock_list.html
        { path: 'stock_list/delivery_request/:transactionNumber', component : OrderTransferDeliveryRequestListComponent },   //  stock_delivery_request.html  
        { path: 'receive_registration', component : StockReceivingRegistrationComponent },    //receive_registration.html    
        { path: 'inventory_list', component : InventoryListComponent }    //inventory_list.html
      ]
	}
];

@NgModule({
  imports: [ RouterModule.forChild(storeRoutes) ],
  exports: [ RouterModule ]
})
export class StoreRoutingModule{ }
