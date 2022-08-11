import { Component,Input, Output, EventEmitter, SimpleChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';
import { OrderStatusEnum } from '@models/enums/order-status-enum.model';
import { AppConstants } from '@components/common/app-constants/app-constants';
declare var jquery:any;
declare var $:any;

@Component({
	selector: 'app-advance-orders-details',
	templateUrl: './advance-orders-details.html'
})


export class AdvanceOrderDetailsComponent {
	

	@Input() showSaveBtn : boolean;
	@Input() details : any;
	@Input() assignment : any;
	@Input() updateForm : FormGroup
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	aoStatus : any;
	isChange = false;
	changeStatusReason : any;

	successMessage : string;	
	errorMessage : any;

	orderStatus : any = [];

	constructor(
		private _apiBaseService : ApiBaseService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{
		this.loadDropDown();
		
	}

	get advanceOrderDetails(): FormArray{
        return this.updateForm.get('advanceOrderDetails') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.updateForm.value;

		this.changeStatusReason = $('#changeStatusReason').val()
		formData.changeStatusReason = this.changeStatusReason
		console.log(formData);


		this._requestService.action = "transactions/approverequests/advanceorders";

		this._requestService.updateRecord(formData.id,formData)
		.subscribe(successCode =>{
			this.updatePage.emit("loadPageRecord");
			if(formData.requestStatus == 3)
			{
				this.successMessage = AppConstants.AdvanceOrderCancelMessage;
			}
			else
			{			
				this.successMessage = AppConstants.AdvanceOrderUpdateSuccessMessage;
			}
            
            this.errorMessage = null;      

            $("#btnSave").hide();
            $("#btnCancel").hide(); 
		},
		error =>{
			this.errorMessage = this._commonViewService.getErrors(error);
			this.successMessage = null;
		});
	
	}


	onSelectStatus(value : any)
    {
		this.aoStatus = value;
		console.log(this.aoStatus);

		if( value != 1 && value !=3){
			this.isChange = false;
		}
		else
		{
			this.isChange = true;
			console.log(this.orderStatus)
		}
		
	}


	onCancel() {
		// this.updateForm.controls.requestStatus.setValue(3);
		$("#saveModal").modal("hide");
		let formData = this.updateForm.value;
		console.log(formData);

		if(formData.orderStatus != OrderStatusEnum.Incomplete)
		{
	
			$("#updateForm :input").prop("disabled", true);
			$("#closeBtn").prop("disabled", false);
		}
		this.onSubmit(formData);
	}

	private loadDropDown(): void
	{
		this._commonViewService.getCommonList("orderstatus",true).subscribe(dll => { 
			this.orderStatus = dll; 
			this.orderStatus = this.orderStatus.filter(p => p.value != OrderStatusEnum.Incomplete)

		});     

	}


	ngOnChanges(changes : SimpleChanges)
	{
		this.isChange = false;
		$("#btnSave").show(); 
		$("#btnCancel").show(); 
		$("#updateForm :input").prop("disabled", false);
		$("#saveBtn").prop("disabled", false);

		this.successMessage = null;
		this.errorMessage = null;
	}

}