import { Component, OnInit  } from '@angular/core';
import { PageModuleService } from '@services/common/pageModule.service';
import { AuthenticationService } from '@services/auth/authentication.service';

import { Router } from '@angular/router';
import { RequestService } from "@services/request.service";

import { LoadingIndicatorService} from '@services/loading-indicator.service';

import * as signalR from '@aspnet/signalr';
import { CookieService } from 'ngx-cookie-service';
import { ApiBaseService } from '@services/api-base.service';
import { BaseComponent } from '@components/common/base.component';

@Component({
	selector: 'app-main-base',
	templateUrl: './main-base.html'
})

export class MainBaseComponent extends BaseComponent implements OnInit   {

	public username : string;
	public userType : number;
	public assignedAt : string;
	public assignment : any;
	msgs: any = [];

	search : string;

	  notification: any = {};
	  loading : boolean = false;


  constructor(
			private pageModuleService: PageModuleService,
			private _auth: AuthenticationService,
			private router: Router,
			private _requestService: RequestService,
			private _loadingIndicatorService : LoadingIndicatorService,
			private _cookieService : CookieService,
			private _apiBaseService : ApiBaseService
		) 
	{ 
		super();
    //   var event = this._requestService.eventSource('dashboard/main/sse/summary');
    //   event.onmessage = (response => {
    //     var data = JSON.parse(response['data']);
    //     this.notification = data;
    //   });

    //   event.onerror = (error => {

    //   });
		this.getNotification();
		this.pageModuleService.loadScripts();
		this.username = this.pageModuleService.username;
		this.userType = parseInt(this.pageModuleService.userType);
		this.assignedAt = this.pageModuleService.assignedAt;
		this.assignment = this.pageModuleService.assignment;
		_loadingIndicatorService.onLoadingChanged
        .subscribe(isLoading => this.loading = isLoading);


	}

	ngOnInit() {
		var assign = this._cookieService.get("assignment");
		//Base component function for getting notification
		this.startSignalRListerner(assign);

		if(this.assignment != 1)
		{
			this.router.navigate(["/Error401"]);
		}		
	}


	onConnection()
	{
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
			this.msgs.push({ severity: type, summary: payload });
			this.getNotification();
		  });
  
	}



	logoutUser() {
		this._hubConnection.stop();
		this._auth.logout();
	}

	getNotification(){
		this._apiBaseService.action = 'dashboard/main/sse/summary';


		this._apiBaseService.getList().subscribe(
			data => {
				this.notification = data;
			}
		);

	}

	onSearch() {

		if (this.search != '') {
			this.router.navigate(['/Main/store_inventory_list'], { queryParams : { keyword : this.search} });
		}
		
	}	
}
