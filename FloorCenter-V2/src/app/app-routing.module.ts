import { NgModule }      		from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageModuleService } from '@services/common/pageModule.service';

import { LoginComponent } from '@components/common/login/login.component';
import { Error401Component } from '@components/common/error-page/error401/error-401.component';
import { Error404Component } from '@components/common/error-page/error404/error-404.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'Error401', component: Error401Component },
  { path: 'login', component: LoginComponent },
  { path: '404', component: Error404Component},
  {
	   path: 'Logistics',
      //  loadChildren: () => import('./modules/logistics/logistics.module').then(mod => mod.LogisticsModule)
      loadChildren: '@modules/logistics/logistics.module#LogisticsModule'

	},
	{
	   path: 'Main',
      //  loadChildren: () =>  import('./modules/main/main.module').then(mod => mod.MainModule)
       loadChildren: '@modules/main/main.module#MainModule'
	},
	// {
	//    path: 'Marketing',
  //      loadChildren: () => import('./modules/marketing/marketing.module#MarketingModule').then(mod => mod.Marke)
	// },
	{
	   path: 'Store',
      //  loadChildren: () => import('./modules/store/store.module').then(mod => mod.StoreModule)
      loadChildren: '@modules/store/store.module#StoreModule'
      
	},
	{
	   path: 'Warehouse',
      //  loadChildren: () => import('./modules/warehouse/warehouse.module').then(mod => mod.WarehouseModule)
      loadChildren: '@modules/warehouse/warehouse.module#WarehouseModule'
	},
  { path: "**",redirectTo:"404"}
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