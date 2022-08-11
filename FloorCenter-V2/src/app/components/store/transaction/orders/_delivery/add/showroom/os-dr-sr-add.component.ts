import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';
import { RequestService } from '@services/request.service';

import { CommonViewService } from '@services/common/common-view.service';

@Component({

	selector: 'app-os-dr-sr-add',
	templateUrl: 'os-dr-sr-add.html'
})



export class OrderStockDeliveryRequestShowroomAddComponent{

	@Output()updateList : EventEmitter<any> = new EventEmitter<any>();

	@Input()stockItemDetails : any;
	@Input()showRoomForm : FormGroup;
	@Input()drLabel : any;

	successMessage : string;
	errorMessage : any;
	constructor(
		private _orderStockService : OrderStockService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService,)
	{

		


	}


	get ShowroomDeliveries(): FormArray{
		return this.showRoomForm.get("ShowroomDeliveries") as FormArray;
	}

	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
          	this.hideSubmitBtn("show");
	}


	onSubmit(){
		let formValue = this.showRoomForm.value;
		this._requestService.action = 'transactions/orders/delivery/showroom';

		this._requestService.updateRecord(formValue.Id,formValue)
			.subscribe(successCode =>{
				this.successMessage = "Showroom Delivery Succesfully Added.";
				this.errorMessage = null;
				this.updateList.emit(successCode);
				this.hideSubmitBtn("hide");
			},
			error => {
				this.errorMessage = this._commonViewService.getErrors(error);
				this.successMessage = null;
			});
        // this._orderStockService.newRecord(formValue)
        // .subscribe(successCode =>{
  
        //   	this.successMessage = "Showroom Delivery Succesfully Added.";
        //   	this.errorMessage = null;
        //   	this.updateList.emit(successCode);
        //   	this.hideSubmitBtn("hide");

        // },
        // error =>{
        //     this.errorMessage = this._commonViewService.getErrors(error);
        //     this.successMessage = null;
        // });
	
	}



	ngOnChanges(changes : SimpleChanges)
	{

		this.successMessage = null;
		this.errorMessage = null;
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

	



	
}