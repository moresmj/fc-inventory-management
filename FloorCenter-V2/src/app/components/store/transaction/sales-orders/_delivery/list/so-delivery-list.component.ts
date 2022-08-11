import { Component, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { SalesOrderService } from '@services/sales-order/sales-order.service';

import { ActivatedRoute } from '@angular/router';
import { CustomValidator } from '@validators/custom.validator';
import 'rxjs/add/operator/map';


@Component({
	selector: 'app-so-delivery-list',
	templateUrl: './so-delivery-list.html'
})

export class SalesOrderDeliveryListComponent {
	
	salesId : number;
	details : any;
	salesDeliveryDetails : any;

    currentDate: any = new Date().toISOString().substring(0,10);

    newForm : FormGroup;

	constructor(
			private route: ActivatedRoute, 
			private fb: FormBuilder,
			private _salesOrderService : SalesOrderService
		) 
	{
		this._salesOrderService.action = "/delivery";
	 	this.salesId = route.snapshot.params['id'];
	 	this.getSalesDetails();
	}

	reloadRecord(event : any){
		this.getSalesDetails();
	}

	createForm() {

		let details = this.details;

		this.newForm = this.fb.group({
			deliveryDate : new FormControl(this.currentDate.substring(0,10),Validators.required),
			siNumber : new FormControl(details.siNumber,Validators.required),
			orNumber : new FormControl(details.orNumber,Validators.required),
			drNumber : new FormControl(details.drNumber),
			clientName : new FormControl(details.clientName,(details.deliveryType == 2) ? Validators.required : null),
			address1 : new FormControl(details.address1),
			address2 : new FormControl(details.address2),
			address3 : new FormControl(details.address3),
			contactNumber : new FormControl(details.contactNumber,(details.deliveryType == 2) ? Validators.required : null),
			remarks : new FormControl(''),
			deliveryType : new FormControl(details.deliveryType),
			clientDeliveries : this.fb.array([])
		});
	}

	getSalesDetails() {
		this._salesOrderService.getListWithID(this.salesId)
			.subscribe( details => { 
				this.details = details;

				console.log(this.details);

				// Get Remaining items for delivery request
				for (let i = 0; i <  this.details["soldItems"].length; i++ ) {		
					let item = this.details["soldItems"][i];
					let remainingQuantity = item["quantity"] - this.getTotalQuantityOfTheItemDelivered(item["itemId"]);
					this.details["soldItems"][i].quantity = remainingQuantity
				}
			});
	}

	addClientDelivery() {

		this.getSalesDetails()
		this.createForm();

		for (let i = 0; i <  this.details["soldItems"].length; i++ ) {
			
			let item = this.details["soldItems"][i];

			const control = <FormArray>this.newForm.controls['clientDeliveries'];
	        let newItemRow = this.fb.group({
		            stSalesId : new FormControl(item["stSalesId"]),
		            stSalesDetailId : new FormControl(item["id"]),
		            itemId : new FormControl(item["itemId"]),	  
		            quantity : new FormControl(item["quantity"],[Validators.required,CustomValidator.deliveredQuantity])	                       
	        })
			control.push(newItemRow);
		}


	}

	getTotalQuantityOfTheItemDelivered(itemId : number) : number {

		let totalDeliveries = 0;
		let deliveries = this.details["deliveries"];

		for (let a = 0; a < deliveries.length; a++) {
			
			let dItem = deliveries[a]["items"];
			for (let b = 0; b < dItem.length; b++) {
				if (dItem[b]["id"] == itemId) {
					totalDeliveries = totalDeliveries + dItem[b]["quantity"];
				}
			}
		}

		return totalDeliveries;
	}


	onBtnDetailsClick(data : any) {
		data["deliveryDate"] = data["deliveryDate"].substring(0,10)
		this.salesDeliveryDetails = data;
	}
	

}