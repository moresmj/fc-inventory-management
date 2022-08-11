import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';
import { Dropdown } from '@models/common/dropdown.model';
import { ErrorMessage } from '@models/common/error.model';
import { Observable } from 'rxjs/Observable';
import { environment } from '@environments/environment';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


@Injectable()
export class CommonViewService{
	
	private base_url = environment.apiBaseUrl;

	constructor(private http: Http) {

	}


	getCommonList(param : string, isEnum = false): Observable<Dropdown[]>
	{
		// isEnum [true]  = Fetch record on Modules.
		// isEnum [false] = Fetch record on Enums.
		let url = "";
		if (isEnum) {
			url = this.base_url + param + "/all";			// OUTPUT: http://[baseUrl]/[param]/all
		}
		else{
			url = this.base_url + param;					// OUTPUT: http://[baseUrl]/[param]
		}

		return this.http.get(url,{headers:this.getUrlEncodedHeaders() })
						.map(this.extractData)
						.catch(this.HandleError);
	}


	getErrors(errMessage : any): string[]
	{
		let _ErrorMessage = new Array<string>();

		for(let key in errMessage)
		{

			_ErrorMessage.push(errMessage[key][0]);
		}
		return _ErrorMessage;

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