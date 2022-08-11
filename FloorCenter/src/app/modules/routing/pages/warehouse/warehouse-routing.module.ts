import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WarehouseBaseComponent }  from '@baseComponents/warehouse/warehouse-base.component';
import { DashboardComponent } from '@warehouse/Dashboard/dashboard.component';
import { DeliveryRequestListComponent } from '@warehouse/delivery-request-management/list/delivery-request-list.component';
import { StockRegistrationComponent } from '@w_srm/registration/stock-regist/stock-registration.component';
import { RecievedListComponent } from '@w_srm/recieved-list/list/recieved-list.component';
import { ImportPhysicalCountComponent } from '@w_im/import-physical-count/ipc.component';

const warehouseRoutes: Routes = [
	{ 
	  path: '',
      component: WarehouseBaseComponent,
      children: [
      	{ path: 'dashboard', component: DashboardComponent },
        { path: 'delivery_list' ,component:DeliveryRequestListComponent },
        { path: 'stock_registration', component:StockRegistrationComponent },
        { path: 'stock_recieved_list', component:RecievedListComponent},
        { path: 'import_physical_count', component:ImportPhysicalCountComponent}
      ]
	} 
];

@NgModule({
  imports: [ RouterModule.forChild(warehouseRoutes) ],
  exports: [ RouterModule ]
})
export class WarehouseRoutingModule{ }
