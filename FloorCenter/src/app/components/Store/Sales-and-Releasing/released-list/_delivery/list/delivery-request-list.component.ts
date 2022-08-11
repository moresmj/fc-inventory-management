import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'app-delivery-request-list',
    templateUrl: './delivery-request-list.html'
})

export class DeliveryRequestListComponent implements OnInit {

	transactionNumber: string;
  	private sub: any;

	constructor(
			private route: ActivatedRoute,
    		private router: Router,
		)  { }
	
	ngOnInit() {
		this.sub = this.route.params.subscribe(params => 
			{ 
				this.transactionNumber = params['transactionNumber'];
				console.log(this.transactionNumber);
			});    
    }

    ngOnDestroy() {
    	this.sub.unsubscribe();
  	}   
}