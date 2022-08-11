import { Component,OnInit, OnDestroy } from '@angular/core';
import { RequestService } from '@services/request.service';
import { ApiBaseService } from '@services/api-base.service';
import { environment } from 'environments/environment';
import * as EventSource from "EventSource";
import { CookieService } from 'ngx-cookie-service';
import * as signalR from '@aspnet/signalr';
import { BaseComponent } from '@components/common/base.component';

@Component({
	selector : 'app-dashboard',
	templateUrl : './dashboard.html'
})

export class DashboardComponent extends BaseComponent implements OnInit, OnDestroy{

  ss : any;
  details: any;
  test: any;

  // private _hubConnection: signalR.HubConnection;


  constructor(private _requestService : RequestService,
              private _apiBaseService : ApiBaseService,
              private _cookieService : CookieService
              )
	{
    super()

    // this._requestService.action = "dashboard/main/stockssummary";
    // this._requestService.getList()
    //   .subscribe(data =>{
    //     this.ss = data;
    // });
    // this._requestService.action = "dashboard/main/sse/summary";
    // this._requestService.getList()
    //     .subscribe(data =>{
    //       this.test = data;
    //     });

    


    // event.onerror = (error => {
    // });

    this.getNotification();
  }

  ngOnInit() {
    var assign = this._cookieService.get("assignment");

    this.startSignalRListerner(assign)
	
	
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
		 
          console.log("success");
          this.getNotification();
		});
  }
  
  ngOnDestroy() {
		
		this._hubConnection.stop();

	}
  

  getNotification()
	{
		this._apiBaseService.action = "dashboard/main/sse/summary";

		this._apiBaseService.getList().subscribe(
			p => {
        this.ss = p;
			}
    );
    

    this._apiBaseService.action = 'dashboard/main/dashboard/summary';

		this._apiBaseService.getList().subscribe(
			p => {
        this.details = p;
			});

	}

}
