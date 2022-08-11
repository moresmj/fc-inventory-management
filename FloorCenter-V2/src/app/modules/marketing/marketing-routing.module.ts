import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@library/_guards/auth.guard';

import { MarketingBaseComponent }  from '@baseComponents/marketing/marketing-base.component';
import { DashboardComponent } from '@marketing/dashboard/dashboard.component';

const marketingRoutes: Routes = [
	{ 
	    path: '',
      component: MarketingBaseComponent,
      canActivate: [AuthGuard],
      children: [
        { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      	{ path: 'dashboard', component: DashboardComponent },
      ]
	}
];

@NgModule({
  imports: [ RouterModule.forChild(marketingRoutes) ],
  exports: [ RouterModule ]
})
export class MarketingRoutingModule{ }
