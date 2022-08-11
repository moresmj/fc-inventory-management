import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { Warehouse } from '@models/warehouse/warehouse.model';
import { environment } from 'environments/environment';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';



@Injectable()
export class WarehouseService{

	private warehouse_url = environment.apiBaseUrl + "warehouses";

	 private inven_url = environment.apiBaseUrl + 'warehouseinventories';

	constructor(private http: Http, private _auth : AuthenticationService){}


	getList(): Observable<Warehouse[]>
	{
		return this.http.get(this.warehouse_url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}



	addWarehouse(wh: Warehouse): Observable<Response>{

		return this.http
		.post(this.warehouse_url, JSON.stringify(wh), {headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);


	}

	updateWarehouse(id: number , wh:Warehouse ): Observable<Response>{
		wh.id = id;
		return this.http
		.put(this.warehouse_url+ "/" + id, JSON.stringify(wh),{headers:this.getUrlEncodedHeaders() })
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

        return body;
    }
	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
	    headers.append('Authorization', 'Bearer ' + this._auth.token);
	    return headers;
    }
    

}