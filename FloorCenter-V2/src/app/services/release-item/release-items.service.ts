import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers,URLSearchParams } from '@angular/http';
import { ReleaseItems	 } from '@models/release-items/release-items.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';


@Injectable()
export class ReleaseItemsService{



	 private r_items_from_orders_url = environment.apiBaseUrl + 'transactions/receiveitems/fromorders';
	 private r_items_url = environment.apiBaseUrl + 'transactions/releaseitems';

	constructor(private http: Http,
		private _auth : AuthenticationService){}


	getList(): Observable<ReleaseItems[]>
	{
		return this.http.get(this.r_items_url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getListWithParam(param : RequestOptions): Observable<ReleaseItems[]>
	{
		  let myParams = new URLSearchParams();
		  


		return this.http.get(this.r_items_url,{params: param,headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}

	getRecordWithId(id: number): Observable<ReleaseItems[]>
	{
		return this.http.get(this.r_items_url+ "/" +id ,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	updateRecord(id: number , wh:ReleaseItems,action:string ): Observable<Response>{
		wh.id = id;
		return this.http
		.put(this.r_items_url +action+ "/" + id, JSON.stringify(wh),{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	addRecord(pr: any): Observable<Response>{
	//	pr.push("file",image);
		pr["UserId"] = pr["CheckedBy"];
		return this.http
		.post(this.r_items_from_orders_url, JSON.stringify(pr), {headers: this.getUrlEncodedHeaders()})
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
	    return headers;
    }
    

}