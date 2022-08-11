import { Component,Input, Output, EventEmitter, SimpleChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';


@Component({
	selector: 'app-change-tonality-details',
	templateUrl: './change-tonality-details.html'
})

export class ApproveRequestChangeTonalityDetailsComponent {
	

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

	get orderedItems(): FormArray{
        return this.updateForm.get('ModifyItemTonalityDetails') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.updateForm.value;
		console.log(formData);

		this._apiBaseService.action = "transactions/approverequests/returns/purchasereturn";
		this._requestService.action = "transactions/approverequests/modifytonality";

		this._requestService.updateRecord(formData.id,formData)
		.subscribe(successCode =>{
			this.updatePage.emit("loadPageRecord");
			if(formData.requestStatus == 3)
			{
				this.successMessage = "Request has been cancelled";
			}
			else
			{
				this.successMessage = "Record Succesfully Updated";
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

		// formData = [];
		// this._requestService.action = "transactions/approverequests/returns/purchasereturncancel";
	    // this._requestService.updateRecord(this.details.id,formData)
        // .subscribe(successCode =>{
        //     this.updatePage.emit("loadPageRecord");

        //     this.successMessage = "Order has been cancelled";
        //     this.errorMessage = null;      

        //     $("#btnSave").hide();
        //     $("#btnCancel").hide(); 
        // },
        // error =>{
        //      this.errorMessage = this._commonViewService.getErrors(error);
        //      this.successMessage = null;
        // });
	}


	ngOnChanges(changes : SimpleChanges)
	{
		$("#btnSave").show(); 
		$("#btnCancel").show(); 

		this.successMessage = null;
		this.errorMessage = null;
	}

}