import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@library/_guards/auth.guard';

import { LogisticsBaseComponent }  from '@baseComponents/logistics/logistics-base.component';
import { DashboardComponent } from '@logistics/dashboard/dashboard.component';
import { OrdersDeliveryListComponent } from '@l_deliveries/orders/list/o-delivery-list.component';
import { SalesDeliveryListComponent } from '@l_deliveries/sales/list/s-delivery-list.component';
import { ReturnDeliveryListComponent } from '@logistics/return-deliveries/list/rd-list.component';

import { ReturnClientDeliveryListComponent } from '@logistics/return-client/list/rc-list.component';

const logisticsRoutes: Routes = [
	{ 
	    path: '',
      component: LogisticsBaseComponent,
      canActivate: [AuthGuard],
      children: [
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      	{ path: 'dashboard', component: DashboardComponent },
        { path: 'deliveries/orders', component: OrdersDeliveryListComponent },
        { path: 'deliveries/sales', component: SalesDeliveryListComponent },
        { path: 'return_delivery_list', component: ReturnDeliveryListComponent },
        { path: 'return_client_delivery_list', component: ReturnClientDeliveryListComponent },        
      ]
	}
];

@NgModule({
  imports: [ RouterModule.forChild(logisticsRoutes) ],
  exports: [ RouterModule ]
})
export class LogisticsRoutingModule{ }
