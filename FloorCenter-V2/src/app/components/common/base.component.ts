import { Injectable,OnInit } from "@angular/core";
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import * as signalR from '@aspnet/signalr';

import { environment } from 'environments/environment';

@Injectable()
export abstract class BaseComponent implements OnInit{
    public now: Date = new Date();
    public _hubConnection: signalR.HubConnection;
    public msgs: any = [];
    public signalRUrl = environment.signalRUrl;
    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

  

    allRecords : any[] = [];
    recordList :   any[] = [];

    Keyword : any = [];
    defaultFilter : any = [];

    sizeList : any = [];
    itemTypeList : any = [];

    paymentModeList : any = [];

    totalRecordMessage : string;
    pageRecordMessage : string;

    searchSuccess: any;
    errorMessage: any;
    statusMessage : any;
    

    

    constructor()
    {

    }

    ngOnInit(){
        

    }


    public startSignalRListerner(assignment : any){

		this._hubConnection = new signalR.HubConnection(this.signalRUrl + assignment,{ transport: signalR.TransportType.LongPolling});

		this._hubConnection
		  .start()
		  .then(() => {
				this.onConnection();
		  } )
		  .catch(err => 
					{
						console.log('Error while establishing connection :(');
						setTimeout(() => {

							this.startSignalRListerner(assignment);

						}, 3000);
					}
				);


	}

	public onConnection()
	{
		this._hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
			this.msgs.push({ severity: type, summary: payload });
            // this.getNotification();
            console.log("base")
            return true;
		  });
  
	}

}