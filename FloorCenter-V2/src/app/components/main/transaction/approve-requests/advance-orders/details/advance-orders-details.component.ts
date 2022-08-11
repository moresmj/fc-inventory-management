import { Component,Input, Output, EventEmitter, SimpleChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';
import { AppConstants } from '@components/common/app-constants/app-constants';


@Component({
	selector: 'app-advance-orders-details',
	templateUrl: './advance-orders-details.html'
})

export class ApproveRequestAdvanceOrderDetailsComponent {
	

	@Input() showSaveBtn : boolean;
	@Input() details : any;
	@Input() updateForm : FormGroup
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	successMessage : string;	
	errorMessage : any;

	constructor(
		private _apiBaseService : ApiBaseService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{
		
	}

	get advanceOrderDetails(): FormArray{
        return this.updateForm.get('advanceOrderDetails') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.updateForm.value;
		console.log(formData);


		this._requestService.action = "transactions/approverequests/advanceorders";

		this._requestService.updateRecord(formData.id,formData)
		.subscribe(successCode =>{
			this.updatePage.emit("loadPageRecord");
			if(formData.requestStatus == 3)
			{
				this.successMessage = AppConstants.requestCancelledMessage;
			}
			else
			{
				this.successMessage = AppConstants.recUpdateSuccessMessage;
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


		onCancel() {
		
		this.updateForm.controls.requestStatus.setValue(3);
		let formData = this.updateForm.value;
		this.onSubmit(formData);
	}


	ngOnChanges(changes : SimpleChanges)
	{
		$("#btnSave").show(); 
		$("#btnCancel").show(); 

		this.successMessage = null;
		this.errorMessage = null;
	}

}