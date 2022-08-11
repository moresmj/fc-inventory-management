import { Component,ViewChild } from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators } from '@angular/forms';
import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreInventories } from '@models/store-inventories/store-inventories.model';
import { ActivatedRoute } from '@angular/router';

import { OrderTransferDeliveryRequestWADComponent } from '@s_stock_req/order-transfer-list/_delivery/warehouse-delivery-add/ot-dr-warehouse-delivery-add.component';


import 'rxjs/add/operator/map';

@Component({
	selector : 'app-ot-delivery-request-list',
	templateUrl : './ot-delivery-request-list.html'
})

export class OrderTransferDeliveryRequestListComponent {

	transactionNumber : number;
	storeInventoryDetails : any; 

	newWarehouseForm : FormGroup;

	@ViewChild(OrderTransferDeliveryRequestWADComponent)
	private warehouseAdd : OrderTransferDeliveryRequestWADComponent;		

	constructor(
		private route: ActivatedRoute, 
		private _storeInventoriesService : StoreInventoriesService, 
		private fb : FormBuilder
		) 
	{
	 	this.transactionNumber = route.snapshot.params['transactionNumber'];
	 	this.getInventoryDetails();
	}

	getInventoryDetails() {
		this._storeInventoriesService.getListWithID(this.transactionNumber)
			.subscribe( details => { 
				this.storeInventoryDetails = details;
			});
	}

	viewInventoryDetails() {
		this.warehouseAdd["details"](this.storeInventoryDetails);
	}


	createAddWarehouseForm(){
		this.newWarehouseForm = this.fb.group({
			DeliveryDate : new FormControl(''),
		    ItemsToBeDelivered : this.fb.array([]) // here
		});
	}


    addNewRow(data: any){

        const control = <FormArray>this.newWarehouseForm.controls['ItemsToBeDelivered'];
            let newItemRow = this.fb.group({
            STInventoryId : new FormControl(data["id"]),
            ItemId: new FormControl(data["itemId"]),
            Quantity : new FormControl(1,Validators.required)
        })
        control.push(newItemRow);
    }


}