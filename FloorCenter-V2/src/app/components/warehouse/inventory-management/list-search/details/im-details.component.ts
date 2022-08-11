import { Component, Input } from '@angular/core';

@Component({
	selector: 'app-im-details',
	templateUrl: './im-details.html'
})

export class InventoryDetailsComponent {

	@Input() details : any;
	constructor()
	{
			
	}

}

