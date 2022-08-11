import { Component, OnInit, OnDestroy } from '@angular/core';
import { RequestService } from '@services/request.service';
import { Router } from '@angular/router';

import * as signalR from '@aspnet/signalr';
import { ApiBaseService } from '@services/api-base.service';

import { CookieService } from 'ngx-cookie-service';
import { BaseComponent } from '@components/common/base.component';

@Component({
	selector : 'app-dashboard',
	templateUrl : './dashboard.html'
})

export class DashboardComponent extends BaseComponent implements OnInit, OnDestroy {

	// GetStocksSummary
	ss : any;



	// GetWarehouseStockAlerts
	wsa : any;


	ssum : any;

	// private _hubConnection: signalR.HubConnection;

	constructor(private _requestService : RequestService,
				private router: Router,
				private _apiBaseService : ApiBaseService,
				private _cookieService : CookieService)
	{
		super();

		this._requestService.action = "dashboard/warehouse/stockssummary"
		this._requestService.getList().subscribe(details => {
			this.ssum = details;
		});

		this.getNotification();

		// var event = this._requestService.eventSource('dashboard/warehouse/sse/summary');
		// event.onmessage = (response => {
		// 		var data = JSON.parse(response['data']);
		// 		this.ss = data;
		// });
	}

	ngOnInit() {
		var assign = this._cookieService.get("assignment");

		//base component function for notification
		this.startSignalRListerner(assign);
		
	
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		  	this.getNotification();
		});
	}

	ngOnDestroy() {
		
		this._hubConnection.stop();

	}

	onSearch(key : string) {

		let param;

		if (key == 'OnlyAvailableStocks') {
			param = { OnlyAvailableStocks : true };
		}
		else if (key == 'IsOutOfStocks') {
			param = { IsOutOfStocks : true };
		}
		else if (key == 'HasBroken') {
			param = { HasBroken : true };
		}

		this.router.navigate(['/Warehouse/inventory_list'], { queryParams : param });

	}

	onSearch2(key : string) {

		let param;

		if (key == 'PickUp') {
			param = { deliveryType : 1 };
		}
		else if (key == 'Delivery') {
			param = { deliveryType : [2,3] };
		}
		

		this.router.navigate(['/Warehouse/release_list'], { queryParams : param });

	}
	
	
	getNotification()
	{
		this._apiBaseService.action = "dashboard/warehouse/sse/summary";

		this._apiBaseService.getList().subscribe(
			p => {
				// var data = JSON.parse(p['data']);
				this.ss = p;
			}
		);

	}

}
