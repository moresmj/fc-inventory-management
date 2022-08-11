import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers,URLSearchParams } from '@angular/http';
import { ReceiveItems } from '@models/receive-items/receive-items.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';


@Injectable()
export class ReceiveItemsService{



	 private r_items_from_orders_url = environment.apiBaseUrl + 'transactions/receiveitems/fromorders';
	 private r_items_url = environment.apiBaseUrl + 'transactions/receiveitems';

	constructor(private http: Http,
		private _auth : AuthenticationService){}


	getList(): Observable<ReceiveItems[]>
	{
		return this.http.get(this.r_items_url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : any): Observable<ReceiveItems[]>
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


		return this.http.get(this.r_items_url,{params: myParams,headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordWithId(id: number): Observable<ReceiveItems[]>
	{
		return this.http.get(this.r_items_from_orders_url+ "/" +id ,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	updateRecord(id: number , wh:ReceiveItems ): Observable<Response>{
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