import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';

import { AuthenticationService } from '@services/auth/authentication.service';
import { Observable } from 'rxjs/Observable';

import { LoadingIndicatorService } from '@services/loading-indicator.service';

@Injectable()
export class RequestInterceptor implements HttpInterceptor {

	constructor(private _auth : AuthenticationService,
		private _loadingIndicatorService : LoadingIndicatorService) { }


	intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		//request started
		this._loadingIndicatorService.onStarted(req);


		return next.handle(req)
		//request end
		.finally(() => this._loadingIndicatorService.onFinished(req));;
	}
}