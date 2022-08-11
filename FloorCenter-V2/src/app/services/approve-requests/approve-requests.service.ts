import { Injectable } from '@angular/core';
import { Http,Headers, Response,RequestOptions} from '@angular/http';
import { ApproveRequests } from '@models/approve-requests/approve-requests.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()

export class ApproveRequestsService {


	private url = environment.apiBaseUrl + "transactions/approverequests";

	constructor(private http : Http, private _auth : AuthenticationService) {}

	getList(): Observable<ApproveRequests[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<ApproveRequests[]>
	{
		return this.http
		.get(this.url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(approveRequests: ApproveRequests): Observable<Response>
	{
		return this.http
		.post(this.url, JSON.stringify(approveRequests), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , approveRequests: ApproveRequests ): Observable<Response>{

		approveRequests.Id = id;
		let url = this.url+ "/" + id;

		return this.http
		.put(url, JSON.stringify(approveRequests),{headers:this.getUrlEncodedHeaders() })
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





