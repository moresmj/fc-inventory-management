import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { SalesOrder} from '@models/sales-order/sales-order.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()
export class SalesOrderService {

	private url = environment.apiBaseUrl + "transactions/salesorder";
	public action : string;

	constructor(private http: Http, private _auth : AuthenticationService){}

	getList(): Observable<SalesOrder[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithID(id : number): Observable<any>
	{
		let url = this.url + this.action + "/" + id;

		return this.http
			.get(url, {headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<SalesOrder[]>
	{
		return this.http
		.get(this.url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(salesOrder: SalesOrder): Observable<Response>
	{
		return this.http
		.post(this.url, JSON.stringify(salesOrder), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , salesOrder:SalesOrder ): Observable<Response> {

		salesOrder.Id = id;
		let url = this.url + this.action + '/' + id;

		return this.http
		.put(url, JSON.stringify(salesOrder),{headers:this.getUrlEncodedHeaders() })
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
       
        return body;
    }

	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
		headers.append('Authorization', 'Bearer ' + this._auth.token);
		
		var stId = this._auth.storeId === undefined ? '' : this._auth.storeId.toString();
		headers.append('storeId', stId);
		
	    return headers;
    }
    

}