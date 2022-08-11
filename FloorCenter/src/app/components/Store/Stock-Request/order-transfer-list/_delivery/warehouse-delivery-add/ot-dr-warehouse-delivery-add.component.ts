import { Component, Input } from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators } from '@angular/forms';
import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreInventories } from '@models/store-inventories/store-inventories.model';
import 'rxjs/add/operator/map';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

@Component({
	selector : 'app-ot-dr-warehouse-delivery-add',
	templateUrl : './ot-dr-warehouse-delivery-add.html'
})

export class OrderTransferDeliveryRequestWADComponent {
	
	//@Input() storeInventoryDetails : any;


	module : string = "storeRequest";


	itemDetails : StoreInventories[] = [];
	newWarehouseForm : FormGroup;

	successMessage : string;	
	errorMessage : any;

	constructor(private _commonViewService : CommonViewService, private _storeInventoriesService : StoreInventoriesService, private fb : FormBuilder) {

	}

	details(data : any) {
		this.createAddWarehouseForm(data[0].id);
		data.map(record => { this.addNewRow(record.requestedItems); });
	}

	createAddWarehouseForm(id :number){
		this.newWarehouseForm = this.fb.group({
			Id : new FormControl(id),
			DeliveryDate : new FormControl(''),
		    ItemsToBeDelivered : this.fb.array([]) // here
		});
	}


	addNewRow(data: any){
		const control = <FormArray>this.newWarehouseForm.controls['ItemsToBeDelivered'];

		this.itemDetails = data;
        data.map(item => 
        	{  
		        let newItemRow = this.fb.group({
		            STInventoryId : new FormControl(item["id"]),
		            ItemId: new FormControl(item["itemId"]),
		            Quantity : new FormControl(1,Validators.required)
		        })
		        control.push(newItemRow);
        	});
    }

    get ItemsToBeDelivered(): FormArray{
        return this.newWarehouseForm.get('ItemsToBeDelivered') as FormArray;
    }

	onSubmit(data : any) {
		let formData = this.newWarehouseForm.value;
		
	    this._storeInventoriesService.newRecord(formData)
        .subscribe(successCode =>{
            //this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Added";
            this.errorMessage = null; 
            //this.newWarehouseForm.reset();       
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}

}