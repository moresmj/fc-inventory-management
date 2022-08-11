import { Component, Input } from '@angular/core';

@Component({
	selector : 'app-si-details',
	templateUrl : './si-details.html'
})

export class StoreInventoryDetailsComponent {
	
	@Input() details : any;

}