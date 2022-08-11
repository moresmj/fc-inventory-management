import { Component } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { StoreOrder } from '@models/store-order/store-order.model';

import { ActivatedRoute } from '@angular/router';
import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { AppConstants } from '@common/app-constants/app-constants';
import 'rxjs/add/operator/map';

@Component({
	selector : 'app-returns-receive-items-details',
	templateUrl : './returns-receive-items-details.html'
})

export class ReturnsReceiveItemsDetailsComponent {
	
	deliveryId : number;
	//storeOrderId : number;	
	receiveDetails : StoreOrder;
	deliveryDetails : any;

	itemDetails : any = [];

	//serialNumber : string;
	updateForm : FormGroup;

	showSaveBtn : boolean = true;
	successMessage : string;
	errorMessage : string;

	//serialErrorMessage : string;

	constructor(
			private route: ActivatedRoute, 
			private _orderService : ApiBaseService,
			private _commonViewService : CommonViewService,
			private fb : FormBuilder
		) 
	{
		this._orderService.action = "transactions/receiveitems/clientreturn/";
	 	this.deliveryId = route.snapshot.params['id'];
	 	this.getReceiveDetails();
	}

	createForm(details : any) {
		console.log(details);
		this.updateForm = this.fb.group({
			id : new FormControl(this.deliveryId),
			transactionNo: new FormControl(details.transactionNo),
		    clientPurchasedItems : this.fb.array([]) // here
		});
	}

	getReceiveDetails() {
		this._orderService.getRecordWithId(this.deliveryId)
			.subscribe( details => { 
				this.receiveDetails = details;
				console.log(details);
				//this.storeOrderId = details["id"];
				this.createForm(details);

				//  Record will always be 1
				for (let a = 0; a <  details["clientPurchasedItems"].length; a++ ) {
					
					
						
						let purchasedItems = details["clientPurchasedItems"][a];
						this.itemDetails.push(purchasedItems);

						const control = <FormArray>this.updateForm.controls['clientPurchasedItems'];
				        let newItemRow = this.fb.group({
					            id : new FormControl(purchasedItems["id"]),
					            STReturnId : new FormControl(purchasedItems["stReturnId"]),
					            itemId : new FormControl(purchasedItems["itemId"]),	  
								ReceivedRemarks : new FormControl(purchasedItems["remarks"]),
								isTonalityAny : new FormControl(purchasedItems["isTonalityAny"])	                      
				        })
        				control.push(newItemRow);
				}
			});
	}

    get clientPurchasedItems(): FormArray{
        return this.updateForm.get('clientPurchasedItems') as FormArray;
    }

    onSubmit() {
        let formData = this.updateForm.value;
        console.log(formData);

        this._orderService.updateRecord(this.deliveryId,formData)
            .subscribe(successCode =>{
                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.showSaveBtn = false;
                this.errorMessage = null;      
                // this.serialErrorMessage = null;  
            },
            error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
                this.successMessage = null;
                // this.serialErrorMessage = null; 
            });



    }

    onCancel() {
    	$("#saveModal").modal("hide");
    }



}