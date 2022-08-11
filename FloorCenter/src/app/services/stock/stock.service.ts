import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions,Headers } from '@angular/http';
import { environment } from '@environments/environment';
import { Stock } from '@models/stock/stock.model';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()
export class StockService{


	private stock_url = environment.apiBaseUrl +'storerequests/incoming_delivery';




	constructor(private http: Http){}


	getList(): Observable<Stock[]>
	{
		return this.http.get(this.stock_url,{headers:this.getUrlEncodedHeaders()})
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithID(id : number): Observable<Stock>
	{
		return this.http
		.get(this.stock_url, { params : {"Id" : id}, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	private extractData(res: Response){
		let body = res.json();
		return body;
	}

	private HandleError(error: Response | any){
		console.error(error.message || error);
		return Observable.throw(error.json());
	}


	protected getUrlEncodedHeaders(){
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		return headers;
	}
}
