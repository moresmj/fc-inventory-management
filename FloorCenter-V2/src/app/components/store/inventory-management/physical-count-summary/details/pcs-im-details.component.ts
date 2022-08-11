import { Component, Input } from '@angular/core';

@Component({
	selector: 'app-pcs-im-details',
	templateUrl: './pcs-im-details.html'
})

export class PhysicalCountSummaryDetailsComponent {

	@Input() details : any;
	constructor()
	{
			
	}

}

