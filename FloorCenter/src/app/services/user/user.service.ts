import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';

import { User } from '@models/user/user.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class UserService{


	private url = environment.apiBaseUrl + "users";

	constructor(private http : Http) {}

	getList(): Observable<User[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<User[]>
	{
		let search_url = this.url + "search";

		return this.http
		.get(search_url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(data : User): Observable<Response>
	{
		return this.http
		.post(this.url, JSON.stringify(data), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , data : User): Observable<Response>{

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