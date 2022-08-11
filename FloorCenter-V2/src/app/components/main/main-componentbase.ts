import { Component, OnInit ,Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild, Injectable } from '@angular/core';
import { ApiBaseService } from '@services/api-base.service';


import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';


import { RequestService } from '@services/request.service';
import 'rxjs/add/operator/delay';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Injectable()
export class MainComponentBase implements OnInit{


    public res : any;
    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">';

    constructor( private service : ApiBaseService,
        private _commonService : CommonViewService,
         private spinnerService: Ng4LoadingSpinnerService){
      
    }

    ngOnInit(){
       
    }


    public onSubmitAdd(form : any)
    {
        this.spinnerService.show();
        this.service.newRecord(form)
        .subscribe(successCode =>{

             this.res = "Success";
             this.spinnerService.hide();
             return this.res;
           
        },
        error =>{
             this.res = this._commonService.getErrors(error);
             this.spinnerService.hide();
             return this.res;
          
        });
    }

}