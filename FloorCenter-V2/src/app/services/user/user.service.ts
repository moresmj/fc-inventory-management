import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';
import { User } from '@models/user/user.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class UserService{


	private url = environment.apiBaseUrl + "users";
	public action = "";

	constructor(private http : Http, private _auth : AuthenticationService) {}

	getList(): Observable<User[]>
	{
		let url = (this.action === "") ? this.url : `${this.url}/${this.action}`;

		return this.http.get(url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<User[]>
	{
		let url = this.url + "search";

		return this.http
		.get(url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(data : User): Observable<Response>
	{
		let url = (this.action === "") ? this.url : `${this.url}/${this.action}`;

		return this.http
		.post(url, JSON.stringify(data), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , data : User): Observable<Response>{

		data.Id = id;
		let url = (this.action === "") ? this.url : `${this.url}/${this.action}`;
		url = `${url}/${id}`;

		return this.http
		.put(url, JSON.stringify(data),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}



	private HandleError(error: Response | any){
		console.error(error.message || error);
		if(error.status == 401)
		{		
			localStorage.removeItem('currentUser');
			window.location.href = environment.baseUrl +"Error401";
		}
		return Observable.throw(error.json());
	}

 	private extractData(res: Response) {

		let body = null;

		if (res["_body"] != "") {
		   body = res.json();
		}

        return body;
    }

	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
	    headers.append('Authorization', 'Bearer ' + this._auth.token);
	    return headers;
    }

}