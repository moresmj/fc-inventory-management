import { Component } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, FormControl, Validators } from '@angular/forms';

import { OrderService } from '@services/order/order.service';
import { StoreOrder } from '@models/store-order/store-order.model';

import { ActivatedRoute } from '@angular/router';
import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { AppConstants } from '@common/app-constants/app-constants';
import 'rxjs/add/operator/map';

import { RequestService } from '@services/request.service';

@Component({
	selector : 'app-receive-items-details',
	templateUrl : './receive-items-details.html'
})

export class ReceiveItemsDetailsComponent {
	
	deliveryId : number;
	//storeOrderId : number;	
	receiveDetails : StoreOrder;
	deliveryDetails : any;

	itemDetails : any = [];

	//serialNumber : string;
	updateForm : FormGroup;

	showSaveBtn : boolean = true;
	successMessage : string;
	errorMessage: string;

  	currentDate: any = new Date().toISOString().substring(0, 10);

	//serialErrorMessage : string;

	constructor(
			private route: ActivatedRoute, 
			private _orderService : OrderService,
			private _commonViewService : CommonViewService,
			private fb : FormBuilder,
			private _requestService : RequestService,
		) 
	{
		this._orderService.action = "orders/receiveitems";
		this.deliveryId = route.snapshot.params['id'];
		this.updateForm = this.fb.group({
			DeliveryDate: new FormControl(this.currentDate),
			DRNumber: new FormControl('')
		});
		this.getReceiveDetails();
	}

	createForm() {
		this.updateForm = this.fb.group({
          id: new FormControl(this.deliveryId),
          DeliveryDate: new FormControl(this.currentDate, (this.receiveDetails["isVendor"]) ? Validators.required : null),
          DRNumber: new FormControl('', (this.receiveDetails["isVendor"]) ? [Validators.required, Validators.maxLength(50)] : null),
          showroomDeliveries: this.fb.array([]) // here

		});
	}

	getReceiveDetails() {
		this._orderService.getListWithID(this.deliveryId)
			.subscribe( details => { 
				this.receiveDetails = details;
				console.log(details)
				//this.storeOrderId = details["id"];
				this.createForm();

				//  Record will always be 1
				for (let a = 0; a <  details["deliveries"].length; a++ ) {
					
					this.deliveryDetails = details["deliveries"][a];
					let delivery = details["deliveries"][a];
					for (let b = 0; b < delivery["showroomDeliveries"].length; b++) {
						
						let showroom = delivery["showroomDeliveries"][b];
						this.itemDetails.push(showroom);

						const control = <FormArray>this.updateForm.controls['showroomDeliveries'];
				        let newItemRow = this.fb.group({
					            id : new FormControl(showroom["id"]),
					            stOrderId : new FormControl(delivery["stOrderId"]),
					            stDeliveryId: new FormControl(showroom["stDeliveryId"]),
					            stOrderDetailId : new FormControl(showroom["stOrderDetailId"]),
					            itemId : new FormControl(showroom["itemId"]),	  
					            deliveredQuantity : new FormControl(showroom["quantity"],[Validators.required,CustomValidator.deliveredQuantity]),	 
								remarks : new FormControl(showroom["remarks"])	 ,
								IsRemainingForReceiving : new FormControl(showroom["isRemainingForReceiving"]), 
								isTonalityAny: new FormControl(showroom["isTonalityAny"])

				        })
        				control.push(newItemRow);
					}
				}
			});
	}

    get showroomDeliveries(): FormArray{
        return this.updateForm.get('showroomDeliveries') as FormArray;
    }

    onSubmit() {
		this._requestService.action =  "transactions/orders/receiveitems";
		let formData = this.updateForm.value;
		

		this._requestService.updateRecord(this.deliveryId,formData)
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

        // this._orderService.updateRecord(this.deliveryId,formData)
        //     .subscribe(successCode =>{
        //         this.successMessage = AppConstants.recordSaveSuccessMessage;
        //         this.showSaveBtn = false;
        //         this.errorMessage = null;      
        //         // this.serialErrorMessage = null;  
        //     },
        //     error =>{
        //         this.errorMessage = this._commonViewService.getErrors(error);
        //         this.successMessage = null;
        //         // this.serialErrorMessage = null; 
        //     });

    }

    onCancel() {
    	$("#saveModal").modal("hide");
    }



}
