import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { Store } from '@models/store/store.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()
export class StoreService{

	private store_url = environment.apiBaseUrl + "stores";


	constructor(private http: Http){}

	getList(): Observable<Store[]>
	{
		return this.http.get(this.store_url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<Store[]>
	{
		let url = this.store_url + "/search";

		return this.http
		.get(url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(store: Store): Observable<Response>
	{
		return this.http
		.post(this.store_url, JSON.stringify(store), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , store:Store ): Observable<Response>{

		store.Id = id;
		
		return this.http
		.put(this.store_url+ "/" + id, JSON.stringify(store),{headers:this.getUrlEncodedHeaders() })
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