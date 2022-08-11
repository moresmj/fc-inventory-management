// MODULES
import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';
import { HttpModule } from '@angular/http';
import { MarketingRoutingModule } from '@modules/marketing/marketing-routing.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@modules/shared.module';

//import { AuthGuard } from '@library/_guards/auth.guard';

// FILTERS

// VALIDATORS

// SERVICES

// COMPONENTS
import { MarketingBaseComponent }  from '@baseComponents/marketing/marketing-base.component';
import { DashboardComponent } from '@marketing/dashboard/dashboard.component';

@NgModule({
	imports : [
		CommonModule,
		HttpModule,
		MarketingRoutingModule,
		RouterModule,
		SharedModule
	],
	declarations : [
		//AuthGuard,
		MarketingBaseComponent,
		DashboardComponent
	]
})

export class MarketingModule {}