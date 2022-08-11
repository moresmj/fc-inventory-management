import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { WarehouseInventories } from '@models/warehouse/warehouse-inventories.model';
import { environment } from '@environments/environment';

import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';


@Injectable()
export class WarehouseInventoriesService{



	 private inventory_url = environment.apiBaseUrl + 'warehouseinventories';

	constructor(private http: Http){}


	getList(): Observable<WarehouseInventories[]>
	{
		return this.http.get(this.inventory_url,{headers:this.getUrlEncodedHeaders() })
		.map(this.extractData)
		.catch(this.HandleError);
	}


	updateRecord(id: number , wh:WarehouseInventories ): Observable<Response>{
		wh.id = id;
		return this.http
		.put(this.inventory_url+ "/" + id, JSON.stringify(wh),{headers:this.getUrlEncodedHeaders() })
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