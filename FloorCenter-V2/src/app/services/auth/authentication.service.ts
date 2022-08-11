import { Injectable } from "@angular/core";
import { Http, Headers, Response } from "@angular/http";
import { environment } from "environments/environment";

import { CookieService } from "ngx-cookie-service";

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/map";
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";

@Injectable()
export class AuthenticationService {
  public token: string;
  public storesHandled: any;
  public storeId: any;

  private login_url: string = environment.apiBaseUrl + "users/authenticate";

  constructor(
    private http: Http,
    private router: Router,
    private cookieService: CookieService
  ) {
    // set token if saved in local storage
    var currentUser = JSON.parse(localStorage.getItem("currentUser"));
    if (currentUser) {
      this.token = currentUser.token;
      this.storesHandled = currentUser.storesHandled;
      this.storeId = this.cookieService.get("storeId");
      console.log(this.storeId);
    }
  }

  login(credentials: any): Observable<any> {
    this.cookieService.deleteAll();
    return this.http
      .post(this.login_url, JSON.stringify(credentials), {
        headers: this.getUrlEncodedHeaders()
      })
      .map(res => {
        let response = res.json();
        let loginDetails = { assignment: response.assignment };

        if (response) {
          this.token = response.token;
          this.storesHandled = response.storesHandled;

          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem(
            "currentUser",
            JSON.stringify({
              id: response.id,
              username: response.username,
              fullName: response.fullName,
              userType: response.userType,
              assignment: response.assignment,
              storeName: response.storeName,
              warehouseName: response.warehouseName,
              token: response.token,
              storesHandled: response.storesHandled
            })
          );

          this.cookieService.set("username", response.username);
          this.cookieService.set("userType", response.userType);
          this.cookieService.set("assignment", response.assignment);

          if (response.userType === 6) {
            this.storeId = response.storesHandled[0]["id"];
            this.cookieService.set("storeId", response.storesHandled[0]["id"]);
          }

          if (response.storeName != null) {
            this.cookieService.set("assignedAt", response.storeName);
          } else if (response.warehouseName != null) {
            this.cookieService.set("assignedAt", response.warehouseName);
            this.cookieService.set("warehouseId", response.warehouseId);
          }

          loginDetails["status"] = true;
        } else {
          loginDetails["status"] = false;
        }
        return loginDetails;
      })
      .catch(this.HandleError);
  }

  ChangeStore(id: any)
  {
    this.cookieService.delete("storeId");
    this.storeId = id;
    this.cookieService.set("storeId", id.toString());
    



  }

  logout(): void {
    // clear token remove user from local storage to log user out
    this.token = null;
    localStorage.removeItem("currentUser");
    //this.router.navigateByUrl('/login');

    this.cookieService.deleteAll();
  }

  private HandleError(error: Response | any) {
    //console.error(error.message || error);

    let err = null;
    if (error["_body"] != "") {
      err = error.json();
    }

    return Observable.throw(err);
  }

  private extractData(res: Response) {
    let body = null;

    if (res["_body"] != "") {
      body = res.json();
    }

    return body;
  }

  protected getUrlEncodedHeaders() {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    return headers;
  }
}
