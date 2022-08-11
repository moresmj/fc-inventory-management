import { Component, Input } from '@angular/core';

@Component({
	selector : 'app-wi-details',
	templateUrl : './wi-details.html'
})

export class WarehouseInventoryDetailsComponent {
	
	@Input() details : any;

}