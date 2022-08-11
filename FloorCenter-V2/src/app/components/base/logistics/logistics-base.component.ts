import { Component,OnInit } from '@angular/core';
import { PageModuleService } from '@services/common/pageModule.service';
import { AuthenticationService } from '@services/auth/authentication.service';
import { RequestService } from "@services/request.service";

import { Router } from '@angular/router';

import {LoadingIndicatorService } from '@services/loading-indicator.service';


import { ApiBaseService } from '@services/api-base.service';
import { CookieService } from 'ngx-cookie-service';

import * as signalR from '@aspnet/signalr';

import { BaseComponent } from '@common/base.component';



@Component({
	selector: 'app-logistics-base',
	templateUrl: './logistics-base.html'
})

export class LogisticsBaseComponent extends BaseComponent implements OnInit {

	public username : string;
	public assignedAt : string;
	public assignment : any;

	notification: any = {};

	loading : boolean = false;

	constructor(private pageModuleService: PageModuleService, 
		private _auth : AuthenticationService , 
		private _requestService: RequestService,
		private router: Router,
		private _loadingIndicatorService : LoadingIndicatorService,
		private _apiBaseService : ApiBaseService,
		private _cookieService : CookieService,) 
	{ 
	super();
	//   var event = this._requestService.eventSource('dashboard/logistics/sse/summary');
    //   event.onmessage = (response => {
    //     var data = JSON.parse(response['data']);
    //     this.notification = data;
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

		//Base component function for notification
		this.startSignalRListerner(this.assignment);
		
		
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		  this.msgs.push({ severity: type, summary: payload });
		  this.getNotification();
		});


		if(this.assignment != 4)
		{
			this.router.navigate(["/Error401"]);
		}				
	}

	logoutUser() {
		this._hubConnection.stop();
		this._auth.logout();
	}


	getNotification()
	{
		this._apiBaseService.action = "dashboard/logistics/sse/summary";

		this._apiBaseService.getList().subscribe(
			p => {

				this.notification = p;

			}
		);

	}


}
