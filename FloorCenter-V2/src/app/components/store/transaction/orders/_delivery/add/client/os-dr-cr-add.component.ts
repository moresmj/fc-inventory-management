import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';

import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';

import { AppConstants } from '@common/app-constants/app-constants';

import { Dropdown } from '@models/common/dropdown.model';
declare var $jquery: any;
declare var $: any;
@Component({

	selector: 'app-os-dr-cr-add',
	templateUrl: 'os-dr-cr-add.html'
})



export class OrderStockDeliveryRequestClientAddComponent{

	@Output()updateList : EventEmitter<any> = new EventEmitter<any>();

	@Input()stockItemDetails : any;
	@Input()clientForm : FormGroup;
	@Input()clientDetails : any;
	@Input()orisRequired : boolean;

	preferredTimeList : Dropdown[] = [];

	preferredTime : any;
	prefTimeisChecked : boolean = false;

	successMessage : string;
	errorMessage : any;
	constructor(
		private _orderStockService : OrderStockService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{

		  this._commonViewService.getCommonList("preferredtime",true)
						  .subscribe(ddl => { this.preferredTimeList = ddl; });
		
		


	}


	get ClientDeliveries(): FormArray{
		return this.clientForm.get("ClientDeliveries") as FormArray;
	}

	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
          	this.hideSubmitBtn("show");
	}


	onSubmit(){
		let formValue = this.clientForm.value;
		this._requestService.action = 'transactions/orders/delivery/client';

		formValue["preferredTime"] = $('input[name=radio]:checked').val();
		console.log(formValue);



		this._requestService.updateRecord(formValue.Id,formValue)
			.subscribe(successCode =>{
				this.successMessage = AppConstants.clientDeliverySuccessMessage;
				this.errorMessage = null;
				this.updateList.emit(successCode);
				this.hideSubmitBtn("hide");
			},
			error =>{
				this.errorMessage = this._commonViewService.getErrors(error);
            	this.successMessage = null;
			});


        // this._orderStockService.newRecord2(formValue)
        // .subscribe(successCode =>{
  
        //   	this.successMessage = "Client Delivery Succesfully Added.";
        //   	this.errorMessage = null;
        //   	this.updateList.emit(successCode);
        //   	this.hideSubmitBtn("hide");

        // },
        // error =>{
        //     this.errorMessage = this._commonViewService.getErrors(error);
        //     this.successMessage = null;
        // });
	
	}


	hideSubmitBtn(condition : any){
		if(condition == "hide")
		{

			 if(!$('#submit').is(':visible'))
			 {
			 	return;
			 }

			 $('#submit').hide();
		}
		else
		{
			if($('#submit').is(':visible'))
			{
			   return;
			}
			

			$('#submit').show();

		}
	}



    chkEvent(event)
    {
        let value = event.currentTarget.value;
        this.preferredTime = value;
        this.prefTimeisChecked = true;

    }




	ngOnChanges(changes : SimpleChanges)
	{

		this.successMessage = null;
		this.errorMessage = null;
		this.prefTimeisChecked = false;
		$('[name="radio"]').prop('checked', false);
	}

	



	
}