import { NgModule }      		from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageModuleService } from '@services/common/pageModule.service';

import { LoginComponent } from '@components/login/login.component';
import { PrintDrComponent } from '@components/Store/print-dr/print-dr.component';

const routes: Routes = [
  {
     path: 'login',
     component: LoginComponent
  },
	{
	   path: 'Main',
     loadChildren: './modules/pages/main/main.module#MainModule'
	},
	{
	   path: 'Store',
     loadChildren: './modules/pages/store/store.module#StoreModule'
	},
	{
	   path: 'Warehouse',
     loadChildren: './modules/pages/warehouse/warehouse.module#WarehouseModule'
	},
  {
     path: 'print_dr',
     component: PrintDrComponent
  },
];
@NgModule({
  imports: [ 
          RouterModule.forRoot(routes) 
  ],
  exports: [ 
          RouterModule 
  ],
  providers: [PageModuleService]
})
export class AppRoutingModule{ } 