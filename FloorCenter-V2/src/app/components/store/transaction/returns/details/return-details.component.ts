import { Component, Input } from '@angular/core';


@Component({
	selector: 'app-return-details',
	templateUrl: 'return-details.html'
})

export class ReturnDetailsComponent {


	@Input() details : any;

	constructor() {

	}

}