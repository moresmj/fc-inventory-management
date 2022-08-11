import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { Product } from '@models/product/product.model';

import { OrderRequestService } from '@services/order_request/order-request.service.ts';

import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';



@Injectable()
export class ProductService{

	 private product_url = environment.apiBaseUrl + 'items';
	 private warehouse_url = environment.apiBaseUrl + 'warehouseinventories';


	constructor(private http: Http){}


	getList(): Observable<Product[]>
	{
		return this.http.get(this.product_url,{headers:this.getUrlEncodedHeaders()})
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<Product[]>
	{
		let search_url = this.product_url + "/search";

		return this.http
		.get(search_url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordDetails(param : RequestOptions): Observable<Product>
	{
		return this.http
		.get(this.product_url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}
		getRecordDetailsWithSerial(id : any): Observable<Product>
	{
		return this.http
		.get(this.product_url, { params : {"serialNumber" : id}, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

		getRecordDetailsWithId(id : any): Observable<Product>
	{
		return this.http
		.get(this.product_url, { params : {"id" : id}, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	addRecord(pr: Product): Observable<Response>{
	//	pr.push("file",image);
		return this.http
		.post(this.product_url, JSON.stringify(pr), {headers: this.getUrlEncodedHeaders()})
		.map(this.extractData)
		.catch(this.HandleError);
	}

	addRecordStock(pr: any): Observable<Response>{
	//	pr.push("file",image);
		return this.http
		.post(this.warehouse_url, JSON.stringify(pr), {headers: this.getUrlEncodedHeaders()})
		.map(this.extractData)
		.catch(this.HandleError);
	}


	updateRecord(id: number, pr: Product): Observable<Response>{
		pr.id = id;
		return this.http
		.put(this.product_url + "/" + id,JSON.stringify(pr),{headers: this.getUrlEncodedHeaders()})
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





