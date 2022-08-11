import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators,FormArray } from '@angular/forms';

import { TransferReleasingService } from '@services/releasing/transfer-releasing.service';

import { Releasing } from '@models/releasing/releasing.model';

import { CommonViewService } from '@services/common/common-view.service';

import { RequestService } from '@services/request.service';
@Component({
	selector: 'app-t-releasing-update',
	templateUrl: 't-releasing-update.html'
})



export class TransferReleasingUpdateComponent implements OnChanges{

	@Input() updateForm : FormGroup;
	@Input() releasingDetails : any;
	@Input() deliveryItems : any;
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();
	successMessage : any;
	errorMessage : any;


	ngOnChanges(changes: SimpleChanges){
		if(changes["updateForm"])
		{
			this.errorMessage = null;
			this.successMessage = null;
			//$("#updateForm :input").prop("disabled", false);
            $("#btnSave").show(); 
		}
	}


	constructor(
		private fb : FormBuilder,
		private _releasingService: TransferReleasingService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{

	}

	onSubmit()
	{
		let formData = this.updateForm.value;
		
		this._requestService.action = "transactions/releasing/transfer";

		if(formData.deliveryType == 2)
		{
			this._requestService.action = "transactions/releasing/transfer/delivery";
		}
		if(formData.deliveryType == 3)
		{
			this._requestService.action = "transactions/releasing/transfer/showroom";
		}


		this._requestService.updateRecord(formData.id,formData)
		.subscribe(SuccessCode=>{
			console.log("success");

			let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

			this.successMessage ="Request has been Released and Updated";
			this.updatePage.emit(emitValue);
            $("#btnSave").hide(); 
            $("#btnCancel").prop("disabled", false);

		},
		error =>{
			       this.errorMessage = this._commonViewService.getErrors(error);
		});


		// this._releasingService.updateRecord(formData.id,formData,action)
		// .subscribe(SuccessCode=>{
		// 	console.log("success");

		// 	let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

		// 	this.successMessage ="Request has been Released and Updated";
		// 	this.updatePage.emit(emitValue);
        //     $("#btnSave").hide(); 
        //     $("#btnCancel").prop("disabled", false);

		// },
		// error =>{
		// 	       this.errorMessage = this._commonViewService.getErrors(error);
		// });

	}



	 get releaseItems(): FormArray{
        return this.updateForm.get('releaseItems') as FormArray;
    }









	
}