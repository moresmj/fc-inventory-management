import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { StoreInventories } from '@models/store-inventories/store-inventories.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()
export class StoreRequestService{

	private url = environment.apiBaseUrl + "StoreRequest/for_delivery";

	constructor(private http: Http){}

	getList(): Observable<StoreInventories[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithID(id : number): Observable<StoreInventories>
	{

		this.url = this.url + "/" + id;

		return this.http
		.get(this.url, { params : {"Id" : id}, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	newRecord(id: number, StoreInventories: StoreInventories): Observable<Response>
	{
		this.url = this.url + "/add/" + id;

		return this.http
		.post(this.url, JSON.stringify(StoreInventories), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , StoreInventories:StoreInventories ): Observable<Response>{

		this.url = this.url + "/add/" + id;
		
		return this.http
		.put(this.url, JSON.stringify(StoreInventories),{headers:this.getUrlEncodedHeaders() })
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