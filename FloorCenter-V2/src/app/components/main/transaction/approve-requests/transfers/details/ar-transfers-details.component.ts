import { Component,Input, Output, EventEmitter, SimpleChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';

@Component({
	selector : 'app-ar-transfers-details',
	templateUrl : './ar-transfers-details.html'
})

export class ApproveRequestTransfersDetailsComponent {

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
		this._apiBaseService.action = "transactions/approverequests/transfers";
	}

	get transferredItems(): FormArray{
        return this.updateForm.get('transferredItems') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.updateForm.value;

		this._requestService.action = "transactions/approverequests/transfers";


		this._requestService.updateRecord(formData["id"],formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;      

            $("#btnSave").hide();
            $("#btnCancel").hide(); 
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });

	    // this._apiBaseService.updateRecord(formData["id"],formData)
        // .subscribe(successCode =>{
        //     this.updatePage.emit("loadPageRecord");

        //     this.successMessage = "Record Succesfully Updated";
        //     this.errorMessage = null;      

        //     $("#btnSave").hide();
        //     $("#btnCancel").hide(); 
        // },
        // error =>{
        //      this.errorMessage = this._commonViewService.getErrors(error);
        //      this.successMessage = null;
        // });
	}


	onCancel() {
		let formData = this.updateForm.value;
		console.log(formData);
		this._requestService.action = "transactions/approverequests/transfers";

		for(let i = 0;i < formData.transferredItems.length; i++)
		{
			formData.transferredItems[i].approvedQuantity = 0;
		}

	    this._requestService.updateRecord(formData["id"],formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Order has been cancelled";
            this.errorMessage = null;      

            $("#btnSave").hide();
            $("#btnCancel").hide(); 
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}

	ngOnChanges(changes : SimpleChanges)
	{
		$("#btnSave").show();
		$("#btnCancel").show(); 

		this.successMessage = null;
		this.errorMessage = null;
	}


}