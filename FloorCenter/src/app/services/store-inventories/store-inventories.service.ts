import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { StoreInventories } from '@models/store-inventories/store-inventories.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()
export class StoreInventoriesService{

	private url = environment.apiBaseUrl + "StoreInventories/search";

	constructor(private http: Http){}

	getList(): Observable<StoreInventories[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithID(id : number): Observable<StoreInventories>
	{
		return this.http
		.get(this.url, { params : {"Id" : id}, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<StoreInventories[]>
	{
		return this.http
		.get(this.url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(StoreInventories: StoreInventories): Observable<Response>
	{
		return this.http
		.post("http://localhost:50893/api/storerequests/for_delivery/add/33", JSON.stringify(StoreInventories), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , StoreInventories:StoreInventories ): Observable<Response>{

		StoreInventories.Id = id;
		
		return this.http
		.put(this.url+ "/" + id, JSON.stringify(StoreInventories),{headers:this.getUrlEncodedHeaders() })
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