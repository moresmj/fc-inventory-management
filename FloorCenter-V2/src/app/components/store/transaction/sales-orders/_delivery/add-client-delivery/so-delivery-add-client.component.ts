import { Component, Input, Output, EventEmitter, SimpleChanges,OnChanges   } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { RequestService } from '@services/request.service';

import { CommonViewService } from '@services/common/common-view.service';
import { AppConstants } from '@common/app-constants/app-constants';
import 'rxjs/add/operator/map';

declare var $: any;

@Component({
	selector: 'app-so-delivery-add-client',
	templateUrl : './so-delivery-add-client.html'
})

export class SalesOrderDeliveryAddClientComponent {

	@Input() details : any;
	@Input() newForm : FormGroup;

	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	preferredTimeList : any= [];
	prefTimeisChecked : boolean = false;

	successMessage : string;
	errorMessage : any;

	constructor(
			private fb: FormBuilder,
            private _requestService : RequestService,
			private _commonViewService : CommonViewService
		) 
	{
		this._requestService.action = "transactions/salesorder/delivery";
		this._commonViewService.getCommonList("preferredtime",true)
                          .subscribe(ddl => { this.preferredTimeList = ddl; } ); 

	}

	get ClientDeliveries(): FormArray{
        return this.newForm.get('clientDeliveries') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.newForm.value;
		formData["preferredTime"] = $('input[name=radio]:checked').val();
		formData["Id"] = this.details.id;

	    this._requestService.updateRecord(this.details.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");

            this.successMessage = AppConstants.deliverySuccessMessage;
            this.errorMessage = null;      
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}


	chkEvent(event)
    {
        this.prefTimeisChecked = true;
        this.onChange();

    }



	onChange(){
          	this.successMessage = null;
          	this.errorMessage = null;
	}

	
	ngOnChanges(changes : SimpleChanges)
	{


		this.prefTimeisChecked = false;
		$('[name="radio"]').prop('checked', false);
	}




}