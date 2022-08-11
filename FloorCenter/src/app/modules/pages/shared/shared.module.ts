import { NgModule } from '@angular/core';
import { CommonModule }   from '@angular/common';

// COMMON COMPONENTS
import { ShowErrorsComponent } from '@components/show-errors/show-errors.component';
import { PagerComponent } from '@pager/pager.component';

@NgModule({
	imports : [
		CommonModule
	],
	declarations : [ 
		PagerComponent,
		ShowErrorsComponent
	],
	exports : [
		PagerComponent,
		ShowErrorsComponent
	]
})

export class SharedModule {}