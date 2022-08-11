import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators,FormArray } from '@angular/forms';

import { RequestService } from '@services/request.service';

import { Releasing } from '@models/releasing/releasing.model';

import { CommonViewService } from '@services/common/common-view.service';
@Component({
	selector: 'app-so-releasing-update',
	templateUrl: 'so-releasing-update.html'
})



export class SalesOrderReleasingUpdateComponent implements OnChanges{

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
		private _requestService: RequestService,
		private _commonViewService : CommonViewService)
	{
        this._requestService.action = "transactions/releasing/salesorder/delivery";
	}

	onSubmit()
	{
		let formData = this.updateForm.value;

		this._requestService.updateRecord(formData.id,formData)
		.subscribe(SuccessCode=>{
			console.log("success");

			let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

			this.successMessage ="Request has been Released and Updated";
			this.updatePage.emit(emitValue);
            $("#btnSave").hide(); 

		},
		error =>{
			       this.errorMessage = this._commonViewService.getErrors(error);
		});

	}



	 get releaseItems(): FormArray{
        return this.updateForm.get('releaseItems') as FormArray;
    }









	
}