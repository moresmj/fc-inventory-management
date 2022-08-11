import { Component,Input, Output, EventEmitter, SimpleChanges, OnChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';

@Component({
	selector: "app-o-delivery-update",
	templateUrl : "./o-delivery-update.html"
})

export class OrdersDeliveryUpdateComponent {
	
	@Input() showSaveBtn : boolean;
	@Input() isClient : boolean;
	@Input() delivery : any;
	@Input() updateForm : FormGroup
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	successMessage : string;	
	errorMessage : any;

	constructor(private _apiBaseService : ApiBaseService, 
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{

	}

	onSubmit() {
		let formData = this.updateForm.value;

		this._requestService.action = "deliveries";
	    this._requestService.updateRecord(this.delivery.id,formData)
        .subscribe(successCode => {
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;  

            $("#btnSave").hide(); 
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}

	onUpdate() {
		let formData = this.updateForm.value;
		formData["id"] = this.delivery.id;

		this._requestService.action = "deliveries/delivery_update";
	    this._requestService.updateRecord(this.delivery.id,formData)
        .subscribe(successCode => {
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;  

            // Hide update button.
            this.delivery["isUpdatable"] = false;
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}



	onChange() {
      	this.successMessage = null;
      	this.errorMessage = null;
      	$("#btnSave").show(); 
	}



	ngOnChanges(changes : SimpleChanges)
	{
		this.successMessage = null;
		this.errorMessage = null;
		$("#btnSave").show(); 
	}


}