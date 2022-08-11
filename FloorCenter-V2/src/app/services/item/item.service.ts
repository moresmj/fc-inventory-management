import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';
import { Item } from '@models/item/item.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class ItemService {


	private url = environment.apiBaseUrl + "items";

	constructor(private http : Http, private _auth : AuthenticationService) {}

	getList(): Observable<Item[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<Item[]>
	{
		let url = this.url + "/search";

		return this.http
		.get(url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(item: Item): Observable<Response>
	{
		return this.http
		.post(this.url, JSON.stringify(item), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , item: Item ): Observable<Response>{

		item.Id = id;
		let url = this.url+ "/" + id;

		return this.http
		.put(url, JSON.stringify(item),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordDetailsWithSerial(id : any): Observable<Item>
	{
		return this.http
			.get(this.url, { params : {"serialNumber" : id}, headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	getItemDetailsWithId(id : any) 
	{
		return this.http
			.get(this.url, { params : {"id" : id }, headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	getItemDetailsWithSerial(serial : any) 
	{
		return this.http
			.get(this.url, { params : {"serialNumber" : serial }, headers:this.getUrlEncodedHeaders() })
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
		return headers;
	}
}





