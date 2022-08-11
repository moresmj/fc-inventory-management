import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import * as EventSource from "EventSource";


@Injectable()

export class RequestService {

	// How to use Action : After declaring the service on the constructor of the component set the action..
	// Action is based on the ACtion Result on the API.

	// API URL 
	// Example : 
	// 		environment.apiBaseUrl =  localhost:4200/ 
	// 		action = users

	// Example Value : localhost:4200/users

	public action : string;
	private url = environment.apiBaseUrl;

	constructor(private http : HttpClient, private _auth : AuthenticationService) {}


	// METHOD : GET
	// DETAILS : Get All Records
	getList(): Observable<any>
	{
		let url = this.url + this.action;

		return this.http
						.get(url, { headers : this.getUrlEncodedHeaders() })
						.map(this.extractData)
						.catch(this.HandleError);
	}

  eventSource(action:string): EventSource {
	var stId = this._auth.storeId === undefined ? '' : this._auth.storeId.toString();
	let url = this.url + action;
	let headers = {
		'Authorization': 'Bearer ' + this._auth.token,
		'storeId' : stId
		
	}
    return new EventSource(url, { headers : headers }); 
  }


	// METHOD : GET
	// DETAILS : Get Record Details by ID
	getRecordWithID(id : number): Observable<any>
	{
		let url = this.url + this.action + "/" + id;

		return this.http
						.get(url, { headers : this.getUrlEncodedHeaders() })
						.map(this.extractData)
						.catch(this.HandleError);
	}



	// METHOD : GET
	// DETAILS : Get Record Details Base on Parameter Field and Value.
	getListWithParam(paramValue : any, paramField : string): Observable<any>
	{
		// this.action ="items";
		let url = this.url + this.action;
		const param = { [paramField] : paramValue };

		return this.http
						.get(url, { params: param, headers : this.getUrlEncodedHeaders() })	
						.map(this.extractData)
						.catch(this.HandleError);
	}


	// METHOD : GET
	// DETAILS : Get Record Details Base on Parameters.
	getListWithParams(param : any): Observable<any>
	{
		let url = this.url + this.action;

		return this.http
						.get(url, { params : param, headers : this.getUrlEncodedHeaders() })
						.map(this.extractData)
						.catch(this.HandleError);
	}




	// METHOD : POST
	// DETAILS : Create new record.
	newRecord(model: any): Observable<Response>
	{
		// this.action ="items";
		let url = this.url + this.action;

		return this.http
						.post(url, JSON.stringify(model), { headers : this.getUrlEncodedHeaders() })
						.pipe()
						.map(this.extractData)
						.catch(this.HandleError);
	}


	// METHOD : PUT
	// DETAILS : Update existing record
	updateRecord(id: number , model: any ): Observable<Response>{

		// this.action ="items";
		let url = this.url + this.action + "/" + id;

		return this.http
						.put(url, JSON.stringify(model), { headers : this.getUrlEncodedHeaders() })
						.map(this.extractData)
						.catch(this.HandleError);
	}


	uploadImage(data : any): Observable<Response>
	{

	    const input = new FormData();
	    input.append("file", data);

		this.action ="items/upload";
		let url = this.url + this.action;

		return this.http
			.post(url, input, { headers:this.getUrlEncodedHeadersForFiles() })
			.map(this.extractData)
			.catch(this.HandleError);
	}


 	private extractData(res: Response) {

		let body = null;

		if (res != null) {
			if (res["_body"] != "" && res["_body"]) {
		  		body = res.json();
			}
			else
			{
				body = res;
			}
		}

        return body;
    }

	private HandleError(error: Response | any){
		console.error(error.message || error);
		return Observable.throw(error.error);
	}


	protected getUrlEncodedHeaders(){

		var stId = this._auth.storeId === undefined ? '' : this._auth.storeId.toString();
		console.log(this._auth.storeId);
		const headers = new HttpHeaders()
			.set("Content-Type", "application/json")
			.set('Authorization', 'Bearer ' + this._auth.token)
			.set('storeId', stId)
		return headers;
	}

	protected getUrlEncodedHeadersForFiles() {
		const headers = new HttpHeaders()
			.set('Authorization', 'Bearer ' + this._auth.token)
		return headers;
	}

}





