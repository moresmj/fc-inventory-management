import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';
import { Inventories } from '@models/inventories/inventories.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class InventoriesService {

	// How to use : Set the action on constructor upon declaring the service.
	// There is two type of action
	// 1. warehouse
	// 2. store
	
	public action : string;
	private url = environment.apiBaseUrl + "inventories/";

	constructor(private http : Http, private _auth : AuthenticationService) {}

	getList(): Observable<Inventories[]>
	{
		let url = this.url + this.action;

		return this.http.get(url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<any>
	{
		let url = this.url + this.action;

		return this.http
		.get(url, { params : param, headers:this.getUrlEncodedHeaders() })
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

	getItemDetailsWithSerial(serial : any): Observable<Inventories>
	{
		let url = this.url + this.action;

		return this.http
			.get(url, { params : {"serialNumber" : serial}, headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	getItemDetailsWithId(id : any) 
	{
		let url = this.url + this.action;

		return this.http
			.get(url, { params : {"id" : id }, headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	newRecord(inventories: Inventories): Observable<Response>
	{

		let crud_url = this.url + this.action;

		return this.http
		.post(crud_url, JSON.stringify(inventories), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , inventories: Inventories ): Observable<Response>{

		let crud_url = this.url + this.action + "/" + id;

		return this.http
		.put(crud_url, JSON.stringify(inventories),{headers:this.getUrlEncodedHeaders() })
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

		var storeId = this._auth.storeId === undefined ? '' : this._auth.storeId.toString();
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		headers.append('Authorization', 'Bearer ' + this._auth.token);
		headers.append('storeId', storeId);
		return headers;
	}
}





