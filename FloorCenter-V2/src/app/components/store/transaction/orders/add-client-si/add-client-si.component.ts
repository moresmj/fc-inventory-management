import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { CommonViewService } from '@services/common/common-view.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';


import { ApiBaseService } from '@services/api-base.service';

import { PagerComponent } from '@common/pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { RequestService } from '@services/request.service';
declare var $jquery: any;
declare var $: any;

@Component({

	selector: 'app-add-client-si',
	templateUrl: 'add-client-si.html'
})



export class AddClientSIComponent{

    @Input()id:number;
    @Input()orderStock : any;
    @Input()addSiForm: FormGroup;

    @Output()updateList : EventEmitter<any> = new EventEmitter<any>();



	   now: Date = new Date();
	   successMessage  : any;
 	      errorMessage: any;


  		searchForm : FormGroup;



  	constructor(private _orderStockService : OrderStockService,
        private fb: FormBuilder,
        private _commonViewService : CommonViewService,
        private _apiBaseService : ApiBaseService,
        private _requestService : RequestService)
    {


   
       
    }


    onUpdate()
    {
     let formValue = this.addSiForm.value;
     console.log(formValue);



     this._requestService.action = 'transactions/orders/addclientsi';


       this._requestService.updateRecord(formValue.Id,formValue)
              .subscribe(successCode  =>{

                this.successMessage = "Client SI Number Successfully assigned";
                this.updateList.emit("loadPageRecord");
                $('#submitBtn').hide();



              },
              error =>{

                this.errorMessage = this._commonViewService.getErrors(error);
                this.successMessage  = null;
              });


    }


    onChange(ch : any){

            this.successMessage = null;
            this.errorMessage = null;
  }


  ngOnChanges(changes : SimpleChanges)
  {

    this.successMessage = null;
    this.errorMessage = null;
     $('#submitBtn').show();
  }

    


   







}