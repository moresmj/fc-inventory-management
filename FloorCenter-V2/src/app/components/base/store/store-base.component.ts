import { Component,OnInit } from '@angular/core';
import { PageModuleService } from '@services/common/pageModule.service';
import { AuthenticationService } from '@services/auth/authentication.service';
import { RequestService } from '@services/request.service';
import { CookieService } from 'ngx-cookie-service';

import { Router } from '@angular/router';
import { BaseComponent } from '@common/base.component';

import { ApiBaseService } from '@services/api-base.service';


import {LoadingIndicatorService } from '@services/loading-indicator.service';
import * as signalR from '@aspnet/signalr';
import { AssignmentEnum } from '@models/enums/assignment-enum.model';

@Component({
	selector: 'app-store-base',
	templateUrl: './store-base.html'
})

export class StoreBaseComponent extends BaseComponent implements OnInit {

	public username : string;
	public assignedAt : string;
	public assignment : any;
	public storesHandled : any;
	public storeId : any;

	notification : any;
	
	notification2 : any;
	
	loading : boolean = false;

	constructor(private pageModuleService: PageModuleService, 
				private _auth : AuthenticationService, 
				private _cookieservice : CookieService, 
				private _requestService : RequestService, 
				private router: Router,
				private _apiBaseService : ApiBaseService,
				private _loadingIndicatorService : LoadingIndicatorService,
				private _cookieService : CookieService,) 
	{ 
		super();
			/*
		  var event = this._requestService.eventSource('dashboard/store/sse/summary');
	      event.onmessage = (response => {
	        var data = JSON.parse(response['data']);
	        this.notification = data;
	      });*/

		//   var event = this._requestService.eventSource('dashboard/store/sse/notif_summary');
	    //   event.onmessage = (response => {
	    //     var data = JSON.parse(response['data']);
	    //     this.notification2 = data;
		//   });

		  this.getNotification();
		  _loadingIndicatorService.onLoadingChanged
		  .subscribe(isLoading => this.loading = isLoading);


	}

	ngOnInit() {
		this.pageModuleService.loadScripts();
		this.username = this.pageModuleService.username;
		this.assignedAt = this.pageModuleService.assignedAt;	
		this.assignment = this.pageModuleService.assignment;
		this.storesHandled = this._auth.storesHandled;
		this.storeId = this._auth.storeId;

		//Base component function
		this.startSignalRListerner(this.assignment);
	
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		  this.msgs.push({ severity: type, summary: payload });
	
		  this.getNotification();
		});
		
		if(this.assignment != AssignmentEnum.Store)
		{
			this.router.navigate(["/Error401"]);
		}	
	}

	logoutUser() {
		this._cookieservice.deleteAll();
		this._hubConnection.stop();
		this._auth.logout();
	}

	setStoreSelected(id : any) {
	
		// this._cookieservice.set("storeId", id.toString());
		this._auth.ChangeStore(id);
		
		window.location.reload();
	}
	onSearch2(fil : any) {

		let param = { filter : fil };

		this.router.navigate(['/Store/orders'], { queryParams : param });
	}

	getNotification()
	{
		this._apiBaseService.action = "dashboard/store/sse/notif_summary";

		this._apiBaseService.getList().subscribe(
			p => {
		
				this.notification2 = p;
	
			}
		);

	}

}
