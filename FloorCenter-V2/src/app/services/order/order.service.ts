import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';
import { StoreOrder } from '@models/store-order/store-order.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class OrderService {

	// How to use : Set the action on constructor upon declaring the service.
	// There is two type of action
	// 1. showroomstock
	// 2. delivery
	// 3. receiveitems

	public action : string;
	private url = environment.apiBaseUrl + "transactions/";

	constructor(private http : Http, private _auth : AuthenticationService) {}

	getList(): Observable<StoreOrder[]>
	{
		let url = this.url + this.action;

		return this.http.get(url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<StoreOrder[]>
	{
		let url = this.url + this.action;

		return this.http
		.get(url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithID(id : number): Observable<StoreOrder>
	{
		let crud_url = this.url + this.action + "/" + id;

		return this.http
			.get(crud_url, {headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	newRecord(storeOrder: StoreOrder): Observable<Response>
	{

		let crud_url = this.url + this.action;

		return this.http
		.post(crud_url, JSON.stringify(storeOrder), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , storeOrder: StoreOrder ): Observable<Response>{

		let crud_url = this.url + this.action + "/" + id;

		return this.http
		.put(crud_url, JSON.stringify(storeOrder),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

 	private extractData(res: Response) {

		let body = null;

		if (res["_body"] != "") {
		   body = res.json();
		}

        return body;
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


	protected getUrlEncodedHeaders(){
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
	    headers.append('Authorization', 'Bearer ' + this._auth.token);
	    headers.append('storeId', this._auth.storeId);
		return headers;
	}
}





