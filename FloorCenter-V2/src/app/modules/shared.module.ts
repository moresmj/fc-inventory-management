import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';

// COMMON COMPONENTS
import { PagerComponent } from '@common/pager/pager.component';
import { Pager2Component } from '@common/pager2/pager2.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import { ShowErrorsComponent } from '@common/show-errors/show-errors.component';

import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';

// DIRECTIVES
import { OnlyNumberDirective } from '@directives/onlynumber.directive';
import { AllowNegativeNumberDirective } from '@directives/allow-negative-number.directive';
import { ContactDirective } from '@directives/contact.directive';
import { PriceDirective } from '@directives/price.directive';


@NgModule({
	imports : [
		CommonModule,
		Ng4LoadingSpinnerModule.forRoot()
	],
	declarations : [ 
		PagerComponent,
		Pager2Component,
		PagerNewComponent,
		ShowErrorsComponent,
		OnlyNumberDirective,
		AllowNegativeNumberDirective,
		ContactDirective,
		PriceDirective
	],
	exports : [
		PagerComponent,
		Pager2Component,
		PagerNewComponent,
		ShowErrorsComponent,
		OnlyNumberDirective,
		AllowNegativeNumberDirective,
		ContactDirective,
		PriceDirective
	]
})

export class SharedModule {}