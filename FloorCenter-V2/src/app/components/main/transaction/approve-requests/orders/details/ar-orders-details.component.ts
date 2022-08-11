import { Component,Input, Output, EventEmitter, SimpleChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';


@Component({
	selector: 'app-ar-orders-details',
	templateUrl: './ar-orders-details.html'
})

export class ApproveRequestOrdersDetailsComponent {
	

	@Input() showSaveBtn : boolean;
	@Input() displayType : string;
	@Input() details : any;
	@Input() updateForm : FormGroup
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	successMessage : string;	
	errorMessage : any;

	isTonalityAny: boolean;

	constructor(
		private _apiBaseService : ApiBaseService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService)
	{
		this._apiBaseService.action = "transactions/approverequests";
		this._requestService.action = "transactions/approverequests";
	}

	get orderedItems(): FormArray{
        return this.updateForm.get('orderedItems') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.updateForm.value;

		for(var i =0; i <this.orderedItems.length; i++)
		{	
			console.log($("#isTonalityAnyItems"+i)[0].checked)
			formData.orderedItems[i].isTonalityAny = $("#isTonalityAnyItems"+i)[0].checked
		}
		
		console.log(formData);
		console.log(this.details.id);
		this._requestService.updateRecord(this.details.id,formData)
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


	    // this._apiBaseService.updateRecord(this.details.id,formData)
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
	chkEvent(event)
	{
		// for(var i =0; i <this.orderedItems.length; i++)
		// {	

		// 	if(event.target.id == "isTonalityAnyItems"+i){

		// 		console.log($("#isTonalityAnyItems"+i)[0].checked)
		// 		this.isTonalityAny = $("#isTonalityAnyItems"+0)[0].checked;
		// 	}

		// }
	}

	onCancel(data : any)
	{
		let formData = this.updateForm.value;
		formData.RequestStatus = 3;
		formData.OrderStatus = 3;


		for(let i = 0;i < formData.orderedItems.length; i++)
		{
			formData.orderedItems[i].approvedQuantity = 0;
		}
		
		this._requestService.updateRecord(this.details.id,formData)
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
		this.isTonalityAny =null;
		$("#btnSave").show(); 
		$("#btnCancel").show(); 

		this.successMessage = null;
		this.errorMessage = null;
	}

}