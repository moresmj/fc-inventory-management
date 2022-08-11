import { Component,Input, ViewChild } from '@angular/core';

@Component({
	selector: 'app-so-delivery-details',
	templateUrl : './so-delivery-details.html'
})

export class SalesOrderDeliveryDetailsComponent {

	@Input() details : any

	constructor() {
		
	}

}