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

	ss : any;
	details : any;
	ssa : any;

	

	constructor(private _requestService : RequestService,
		 private router: Router,
		 private _apiBaseService : ApiBaseService,
		 private _cookieService : CookieService,)
	{
		super();

		this.getNotification();


	    this._requestService.action = "dashboard/store/stockssummary";
	    this._requestService.getList()
	      .subscribe(data =>{
	        this.ss = data;
	    });

		// var event = this._requestService.eventSource('dashboard/store/sse/summary');
		// 	event.onmessage = (response => {
		// 		var data = JSON.parse(response['data']);
		// 		console.log(data);
		// 		this.details = data;
		// });

		 this._requestService.action = "dashboard/store/stockalerts"
			  this._requestService.getList().subscribe(details => {
			   this.ssa = details;
			   console.log(this.ssa);
		});
	
	}


	ngOnInit() {
		var assign = this._cookieService.get("assignment");


		this.startSignalRListerner(assign);
		// this._hubConnection = new signalR.HubConnection("http://localhost:50930/notify?Assignment="+ assign);
		// this._hubConnection
		//   .start()
		//   .then(() => console.log('Connection started!'))
		//   .catch(err => console.log('Error while establishing connection :('));
	
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		 
		  console.log("success");
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

		this.router.navigate(['/Store/inventory_list'], { queryParams : param });

	}
	
	getNotification()
	{
		this._apiBaseService.action = "dashboard/store/sse/summary";

		this._apiBaseService.getList().subscribe(
			p => {
				// var data = JSON.parse(p['data']);
				this.details = p;
			}
		);

	}

}
