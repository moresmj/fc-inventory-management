import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers,URLSearchParams } from '@angular/http';
import { Releasing } from '@models/releasing/releasing.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';


@Injectable()
export class TransferReleasingService{



	 private url = environment.apiBaseUrl + 'transactions/releasing/transfer';
	 

	constructor(private http: Http,
		private _auth : AuthenticationService){}


	getList(): Observable<Releasing[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}
	getListWithParam(param : RequestOptions): Observable<Releasing[]>
	{
		return this.http
		.get(this.url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordWithId(id: number,action: string): Observable<Releasing>
	{
		return this.http.get(this.url+action+id ,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(Releasing: any): Observable<Response>
	{
		return this.http
		.put(this.url+"/delivery/showroom/"+Releasing.Id, JSON.stringify(Releasing), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , wh:Releasing,action : string ): Observable<Response>{
		wh.id = id;
		return this.http
		.put(this.url+ "/"+action + id, JSON.stringify(wh),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newSalesRecord(Sales : any, action : string)
	{
		return this.http
		.post(this.url + "/"+action, JSON.stringify(Sales), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}
/*
	getListWithParam(param : RequestOptions): Observable<Releasing[]>
	{


		  let myParams = new URLSearchParams();
		  if(param != null)
		  {



		  		myParams.set('poNumber', param.poNumber);
		  		myParams.set('drNumber', param.drNumber);
		  		myParams.set('poDateFrom', param.poDateFrom);
		  		myParams.set('poDateTo',param.poDateTo);
		  		myParams.set('drDateFrom', param.drDateFrom);
		  		myParams.set('drDateTo',param.drDateTo);	
		  		myParams.set('itemName',param.itemName);
		  		myParams.set('UserId', param.receiveBy);	  	
		  }


		return this.http.get(this.url,{params: myParams,headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}
	*/

	/*getRecordWithId(id: number): Observable<Releasing[]>
	{
		return this.http.get(this.r_items_from_orders_url+ "/" +id ,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	updateRecord(id: number , wh:Releasing ): Observable<Response>{
		wh.id = id;
		return this.http
		.put(this.r_items_from_orders_url+ "/" + id, JSON.stringify(wh),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	addRecord(pr: any): Observable<Response>{
	//	pr.push("file",image);
		pr["UserId"] = pr["CheckedBy"];
		return this.http
		.post(this.r_items_from_orders_url, JSON.stringify(pr), {headers: this.getUrlEncodedHeaders()})
		.map(this.extractData)
		.catch(this.HandleError);
	}
	*/




	

	private HandleError(error: Response | any){
		if(error.status == 401)
		{		
			localStorage.removeItem('currentUser');
			window.location.href = environment.baseUrl +"Error401";
		}
		console.error(error.message || error);
		return Observable.throw(error.json());
	}

	 private extractData(res: Response) {

  		let body = null;

  		if (res["_body"] != "") {
   				body = res.json();
  		}
       
        console.log(body);
        return body;
    }

	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
	    headers.append('Authorization', 'Bearer ' + this._auth.token);
	    return headers;
    }
    

}