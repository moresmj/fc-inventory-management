import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Item } from '@models/item/item.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Injectable()

export class ItemService {


	private url = environment.apiBaseUrl + 'items';
	private item_url = environment.apiBaseUrl + 'storeinventories/';

	constructor(private http : Http) {}

	getList(): Observable<Item[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<Item[]>
	{
		let search_url = this.url + "search";

		return this.http
		.get(search_url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordDetailsWithCode(Serialnumber : string): Observable<Item>
	{
		return this.http
		.get(this.url, { params : {"Serialnumber" : Serialnumber}, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(data : any): Observable<Response>
	{

		let add_url = this.item_url + "request/add";

		return this.http
		.post(add_url, JSON.stringify(data), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , data : Item): Observable<Response>{

		data.Id = id;

		return this.http
		.put(this.url+ "/" + id, JSON.stringify(data),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}



	private HandleError(error: Response | any){
		console.error(error.message || error);
		return Observable.throw(error.json());
	}

	private extractData(res: Response) {
        let body = res.json();
        //console.log(body);
        return body;
    }

	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
	    return headers;
    }

}