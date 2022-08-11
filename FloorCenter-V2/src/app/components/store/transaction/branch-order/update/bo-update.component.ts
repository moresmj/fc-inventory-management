import { Component,Input, Output, EventEmitter, SimpleChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { BranchOrderService } from '@services/branch-order/branch-order.service';
import { CommonViewService } from '@services/common/common-view.service';

import { RequestService } from '@services/request.service';
import { AppConstants } from '@components/common/app-constants/app-constants';

@Component({
	selector : 'app-bo-update',
	templateUrl : './bo-update.html'
})

export class BranchOrdersUpdateComponent {

	@Input() details : any;
	@Input() updateForm : FormGroup
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	successMessage : string;	
	errorMessage : any;

	constructor(
		private _branchOrderService: BranchOrderService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{ 

	}

	onSubmit(data : any) {
		let formData = this.updateForm.value;

		this._requestService.action = "branchorders";


		this._requestService.updateRecord(this.details.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");

            this.successMessage = AppConstants.recUpdateSuccessMessage;
            this.errorMessage = null;      

            $("#btnSave").hide(); 
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });

        // this._branchOrderService.updateRecord(this.details.id,formData)
        // .subscribe(successCode =>{
        //     this.updatePage.emit("loadPageRecord");

        //     this.successMessage = "Record Succesfully Updated";
        //     this.errorMessage = null;      

        //     $("#btnSave").hide(); 
        // },
        // error =>{
        //      this.errorMessage = this._commonViewService.getErrors(error);
        //      this.successMessage = null;
        // });

	}

	ngOnChanges(changes : SimpleChanges)
	{
		$("#btnSave").show(); 

		this.successMessage = null;
		this.errorMessage = null;
	}


}