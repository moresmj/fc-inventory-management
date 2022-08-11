import { Component, OnInit,OnDestroy } from '@angular/core';
import { RequestService } from '@services/request.service';




import * as signalR from '@aspnet/signalr';

import { ApiBaseService } from '@services/api-base.service';
import { CookieService } from 'ngx-cookie-service';
import { BaseComponent } from '@common/base.component';



@Component({
	selector : 'app-dashboard',
	templateUrl : './dashboard.html'
})

export class DashboardComponent extends BaseComponent  implements OnInit,OnDestroy  {

	details : any;

	constructor(
		private _requestService : RequestService,
		private _apiBaseService : ApiBaseService,
		private _cookieService : CookieService,)
	{
		super();

	//   var event = this._requestService.eventSource('dashboard/logistics/sse/summary');
    //   event.onmessage = (response => {
    //     var data = JSON.parse(response['data']);
    //     this.details = data;
	//   });
	
		this.getNotification();
	}


	ngOnInit() {
		var assign = this._cookieService.get("assignment");


		//Base component function for notification
		this.startSignalRListerner(assign);

		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		  this.getNotification();
		});
	}

	ngOnDestroy() {
		
		this._hubConnection.stop();

	}


	getNotification()
	{
		this._apiBaseService.action = "dashboard/logistics/sse/summary";

		this._apiBaseService.getList().subscribe(
			p => {

				this.details = p;

			}
		);

	}

}
