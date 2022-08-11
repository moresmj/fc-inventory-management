import { EventEmitter, Injectable, Component, NgModule } from '@angular/core'
import {HTTP_INTERCEPTORS, 
    HttpClientModule, 
    HttpClient, 
    HttpEvent, 
    HttpInterceptor, 
    HttpHandler, 
    HttpRequest } from '@angular/common/http';

@Injectable()
export class LoadingIndicatorService{


    onLoadingChanged : EventEmitter<boolean> = new EventEmitter<boolean>();


    /**
     * Stores all currently active request
     */

     private request : HttpRequest<any>[] =[];
     /**
      * Adds request to the storage and notifies observers
      */

      onStarted(req: HttpRequest<any>): void{
          this.request.push(req);
          this.notify();
      }


      /**
       * Removes requset from the storage and notifies observers
       */

       onFinished(req: HttpRequest<any>): void{
           const index = this.request.indexOf(req);
           if(index !== -1){
               this.request.splice(index,1);
           }
           this.notify();
       }

       /**
        * Notifies obervers about wheter there are any request on fly
        */
       private notify(): void{
           this.onLoadingChanged.emit(this.request.length !== 0);
       }

}