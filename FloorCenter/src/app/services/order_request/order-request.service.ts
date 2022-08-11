import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { environment } from '@environments/environment';
import { OrderRequest } from '@models/order_request/order-request.model';



import { Observable } from 'rxjs/Observable';


@Injectable()

export class OrderRequestService{


	private url = environment.apiBaseUrl + 'requests/orders/pending';



	constructor(private http: Http){}



	getList(): Observable<OrderRequest[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	updateRequestForm(id:number,or: any): Observable<Response>{
		return this.http.
		put(this.url +"/update/" + id, JSON.stringify(or),{headers: this.getUrlEncodedHeaders()})
		.map(this.extractData)
		.catch(this.HandleError);
	}



	private HandleError(error : Response | any)
	{
		console.error(error.message || error);
		return Observable.throw(error.json());
	}


	private extractData(res: Response){
		let body = res.json();
		return body;
	}


	private getUrlEncodedHeaders(){
		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		return headers;
	}
}