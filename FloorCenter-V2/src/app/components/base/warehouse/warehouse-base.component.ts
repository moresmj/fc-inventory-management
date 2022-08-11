import { Component,OnInit } from '@angular/core';
import { RequestService } from '@services/request.service';
import { PageModuleService } from '@services/common/pageModule.service' 
import { AuthenticationService } from '@services/auth/authentication.service';

// import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

import { Router } from '@angular/router';

import { CookieService } from 'ngx-cookie-service';



import { LoadingIndicatorService } from '@services/loading-indicator.service';
import * as signalR from '@aspnet/signalr';
import { ApiBaseService } from '@services/api-base.service';
import { BaseComponent } from '@components/common/base.component';


@Component({
	selector: 'app-warehouse-base',
	templateUrl: './warehouse-base.html'
})

export class WarehouseBaseComponent extends BaseComponent  implements OnInit {

	public username : string;
	public assignedAt : string;
	public assignment : any;

	// private _hubConnection: signalR.HubConnection;

	ss : any;
	loading : boolean = false;
	msgs: any = [];
	
	constructor(private _requestService : RequestService,
		private pageModuleService: PageModuleService,
		private _auth : AuthenticationService,
		private router: Router,
		private _loadingIndicatorService : LoadingIndicatorService,
		private _apiBaseService : ApiBaseService,
		private _cookieService : CookieService,
		) 
	{ 
		super()

		// var event = this._requestService.eventSource('dashboard/warehouse/sse/summary');
		// event.onmessage = (response => {
		// 		var data = JSON.parse(response['data']);
		// 		this.ss = data;
		// });

		_loadingIndicatorService.onLoadingChanged
		.subscribe(isLoading => this.loading = isLoading);
		this.getNotification();

	}
	
	ngOnInit() {
		this.pageModuleService.loadScripts();
		this.username = this.pageModuleService.username;
		this.assignedAt = this.pageModuleService.assignedAt;
		this.assignment = this.pageModuleService.assignment;
		if(this.assignment != 2)
		{
			this.router.navigate(["/Error401"]);
		}


		//Base component function
		this.startSignalRListerner(this.assignment);

		
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		  this.msgs.push({ severity: type, summary: payload });
	
		  this.getNotification();
		});


	
	}

	logoutUser() {
		this._auth.logout();
		this._hubConnection.stop();
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
			
				this.ss = p;
			}
		);

	}


}
