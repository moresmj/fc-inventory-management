import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class ApiBaseService {


	public action : string = "";
	private url = environment.apiBaseUrl;
	public loading : boolean  = false;

	constructor(private http : Http, private _auth : AuthenticationService) {}

	getList(): Observable<any>
	{
		let url = this.url + this.action;
		this.loading = true;

		return this.http.get(url,{headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	getListWithParam(param : any): Observable<any>
	{
		let url = this.url + this.action;

		return this.http
			.get(url, { params : param, headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	getRecordWithId(id: number): Observable<any>
	{
		let url = this.url + this.action;
		return this.http.get(url+id ,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(data: any): Observable<Response>
	{
		let url = this.url + this.action;
		this.loading = true;
		return this.http
			.post(url, JSON.stringify(data), {headers:this.getUrlEncodedHeaders() })
			.map(this.extractData)
			.catch(this.HandleError);
	}

	updateRecord(id: number , data: any): Observable<Response>
	{
		data.Id = id;
		let url = this.url + this.action + "/" + id;

		return this.http
		.put(url, JSON.stringify(data),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecordWithoutId( data: any): Observable<Response>
	{
		let url = this.url + this.action;

		return this.http
		.put(url, JSON.stringify(data),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

 	private extractData(res: Response) {

		let body = null;

		if (res["_body"] != "") {
		   body = res.json();
		}
		this.loading = false;
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
		var stId = this._auth.storeId === undefined ? '' : this._auth.storeId.toString();

		let headers = new Headers();
		headers.append('Content-Type', 'application/json');
		headers.append('Authorization', 'Bearer ' + this._auth.token);
		headers.append('storeId', stId);
		return headers;
	}
}





