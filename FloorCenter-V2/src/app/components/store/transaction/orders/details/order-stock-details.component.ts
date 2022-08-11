import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { CommonViewService } from '@services/common/common-view.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';

import { PagerComponent } from '@common/pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { RequestService } from '@services/request.service';
declare var $jquery: any;
declare var $: any;

@Component({

	selector: 'app-order-stock-details',
	templateUrl: 'order-stock-details.html'
})



export class OrderStockDetailsComponent{

    @Input()id:number;
	@Input()orderStock : any;
	@Input()updateForm :FormGroup;
	@Output()updateList : EventEmitter<any> = new EventEmitter<any>();
	poNumber : any;

	successMessage : any;
	
	deliveryModeList : any = [];

	paymentmodeList : any = [];


	now: Date = new Date();
	searchSuccess: any;
	errorMessage: any;



	allRecords : OrderStock[] = [];
	orderStockList:   OrderStock[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;
	searchForm : FormGroup;

	module : string = "order-stock";

	isDealer : Boolean;
	currentUser = JSON.parse(localStorage.getItem("currentUser")).userType;

  	constructor(private _orderStockService : OrderStockService,
		private fb: FormBuilder,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
    {
	   this.load();

	   if(this.currentUser == 6){
		 console.log(this.currentUser )
		 this.isDealer = true
	   }
	}
	
	load(){
		this._commonViewService.getCommonList("deliverytypes",true).subscribe(dll => this.deliveryModeList = dll);
		this._commonViewService.getCommonList("paymentmodes",true).subscribe(dll => this.paymentmodeList = dll);
	
	}

	onUpdate()
	{
		let formValue = this.updateForm.value;
		console.log(formValue);

		this._requestService.action = 'transactions/orders/warehouse/advanceorder/modifydelivery';


		this._requestService.updateRecord(formValue.id,formValue)
			   .subscribe(successCode  =>{
 
				 this.successMessage = "Record Successfully updated";
				 this.updateList.emit("loadPageRecord");
				 $('#submitBtn').hide();

				 if(this.orderStock.poNumber === null)
				 {
					 this.poNumber = successCode;
				 }

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
	
	}



}