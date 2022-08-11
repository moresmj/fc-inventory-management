import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { Warehouse } from '@models/warehouse/warehouse.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';


@Injectable()
export class WarehouseService{

	private warehouse_url = environment.apiBaseUrl + "warehouses";

	 private inven_url = environment.apiBaseUrl + 'warehouseinventories';

	constructor(private http: Http){}


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
		console.error(error.message || error);
		return Observable.throw(error.json());
	}

	  private extractData(res: Response) {
            let body = res.json();
                return body;
            }

	protected getUrlEncodedHeaders(){
	    let headers = new Headers();
	    headers.append('Content-Type', 'application/json');
	    return headers;
    }
    

}