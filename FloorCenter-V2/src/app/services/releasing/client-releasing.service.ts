import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers,URLSearchParams } from '@angular/http';
import { Releasing } from '@models/releasing/releasing.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';


@Injectable()
export class ClientReleasingService{



	 private url = environment.apiBaseUrl + 'transactions/releasing/forclientorder';
	 

	constructor(private http: Http,
		private _auth : AuthenticationService){}


	getList(): Observable<Releasing[]>
	{
		return this.http.get(this.url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}
	getListWithParam(param : RequestOptions): Observable<Releasing[]>
	{
		return this.http
		.get(this.url, { params : param, headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordWithId(id: number,action: string): Observable<Releasing>
	{
		return this.http.get(this.url+action+id ,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newRecord(Releasing: any): Observable<Response>
	{
		return this.http
		.put(this.url+"/delivery/showroom/"+Releasing.Id, JSON.stringify(Releasing), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	updateRecord(id: number , wh:Releasing,action : string ): Observable<Response>{
		wh.id = id;
		return this.http
		.put(this.url+ "/"+action + id, JSON.stringify(wh),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	newSalesRecord(Sales : any, action : string)
	{
		return this.http
		.post(this.url + "/"+action, JSON.stringify(Sales), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
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

	 private extractData(res: Response) {

  		let body = null;

  		if (res["_body"] != "") {
   				body = res.json();
  		}
       
        console.log(body);
        return body;
    }

	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
		headers.append('Authorization', 'Bearer ' + this._auth.token);
		
		var stId = this._auth.storeId === undefined ? '' : this._auth.storeId.toString();
		headers.append('storeId', stId);
		
	    return headers;
    }
    

}