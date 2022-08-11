import { Component,Input, Output, EventEmitter, SimpleChanges,OnInit  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';

import { ActivatedRoute } from '@angular/router';

import { AppConstants } from '@common/app-constants/app-constants';

import { CustomValidator } from '@validators/custom.validator';

import { InventoriesService } from '@services/inventories/inventories.service';

import { PageModuleService } from '@services/common/pageModule.service';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { DISABLED } from '@angular/forms/src/model';


@Component({
	selector: 'app-allocate-advance-orders-details',
	templateUrl: './allocate-advance-orders-details.html'
})

export class AllocateAdvanceOrderDetailsComponent implements OnInit {

	template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

	deliveryId : number;
	deliveryDetails : any;

	newItemDetails : any = {};

	itemDetails : any = [];
	updateForm : FormGroup;

	showSaveBtn : boolean = true;
	successMessage : string;
	errorMessage: string;

	  currentDate: any = new Date().toISOString().substring(0, 10);
	  
	  itemList : any = [];
	  assignment : any;


	//serialErrorMessage : string;


	ngOnInit(): void {
		this.assignment = this.pageModuleService.whId;
		this.getInventoriesWithReserve(this.deliveryId);
		
		
	}

	constructor(
			private route: ActivatedRoute, 
			private _commonViewService : CommonViewService,
			private _requestService : RequestService,
			private _inventoriesService : InventoriesService,
			private fb : FormBuilder,
			private pageModuleService : PageModuleService,
			private spinnerService: Ng4LoadingSpinnerService,
		) 
	{
		this._requestService.action = "transactions/orders/warehouse/advanceorder/details";
		this.deliveryId = route.snapshot.params['id'];
		this.updateForm = this.fb.group({
			DeliveryDate: new FormControl(this.currentDate),
			DRNumber: new FormControl('')
		});

		console.log(this.deliveryId);
		
		this.getadvanceOrder();
	}

	createForm() {
		this.updateForm = this.fb.group({
		  stAdvanceOrderId : new FormControl(this.deliveryId),
          allocateAdvanceOrderDetails: this.fb.array([]) // here

		});
	}

	getadvanceOrder() {
	
		this._requestService.getRecordWithID(this.deliveryId)
			.subscribe( details => { 
				this.deliveryDetails = details;

				
				// this.newItemDetails = new Array(details["advanceOrderDetails"].length);

				

				console.log(details)
				//this.storeOrderId = details["id"];
				this.createForm();

				//  Record will always be 1
				for (let i = 0; i <  details["advanceOrderDetails"].length; i++ ) {
						let aoDetails = details["advanceOrderDetails"][i];
						console.log(aoDetails);

						const control = <FormArray>this.updateForm.controls['allocateAdvanceOrderDetails'];
				        let newItemRow = this.fb.group({
					            stAdvanceOrderId : new FormControl(aoDetails["stAdvanceOrderId"]),
					            itemId : new FormControl(""),		  
								allocatedQuantity : new FormControl(aoDetails["forAllocationQty"]),
								remarks : new FormControl(aoDetails["remarks"]),
								warehouseId : new FormControl(details.warehouseId),
								storeId: new FormControl(details.storeId),
								isCustom: new FormControl(aoDetails["isCustom"]),
								code: new FormControl(aoDetails["itemCode"]),
								sizeId: new FormControl(aoDetails["sizeId"])
							                   
						})
						if(aoDetails.forAllocationQty == 0)
						{
							newItemRow.controls["allocatedQuantity"].disable();
							newItemRow.controls["itemId"].disable();
							newItemRow.controls["remarks"].disable();
						}
        				control.push(newItemRow);
					}
				
			});
	}

	getInventoriesWithReserve(id : any){
		this.spinnerService.show();
		
		this._inventoriesService.action = "warehouse/reserveitems"
                this._inventoriesService.getListWithID(id).subscribe( data => {

																		this.itemList = data;
																		console.log(data);
													
																		this.spinnerService.hide();
                                                                    });
	}

    get allocateAdvanceOrderDetails(): FormArray{
        return this.updateForm.get('allocateAdvanceOrderDetails') as FormArray;
    }

    onSubmit() {
        let formData = this.updateForm.value;
		console.log(formData);
		
		this._requestService.action = "transactions/orders/warehouse/advanceorder/allocate";

        this._requestService.updateRecord(this.deliveryId,formData)
            .subscribe(successCode =>{
                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.showSaveBtn = false;
				this.errorMessage = null;
				this._requestService.action = "transactions/orders/warehouse/advanceorder/details";
				this.getadvanceOrder();      
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
	onChangeValue()
	{
		this.showSaveBtn = true;
	}
	
	onChange(event : any, fGroup : any, ind : any)
	{
		console.log(event);
		console.log(ind);
		
		var selectedItem = this.itemList.find(p => p.itemId == event);

		if(selectedItem === undefined)
		{
			this.deliveryDetails.advanceOrderDetails[ind]["newTonality"] = "";
			this.deliveryDetails.advanceOrderDetails[ind]["newSizeName"] = "";
			this.deliveryDetails.advanceOrderDetails[ind]["newSerialNumber"] = "";
			this.deliveryDetails.advanceOrderDetails[ind]["newReservedQty"] = "";
		}
		else{
			this.deliveryDetails.advanceOrderDetails[ind]["newTonality"] = "Tonality : " + selectedItem.tonality;
			this.deliveryDetails.advanceOrderDetails[ind]["newSizeName"] = "Size : " + selectedItem.sizeName;
			this.deliveryDetails.advanceOrderDetails[ind]["newSerialNumber"] = "Serial : " + selectedItem.serialNumber;
			this.deliveryDetails.advanceOrderDetails[ind]["newReservedQty"] = selectedItem.reserved;
		}
		
		

		console.log(this.newItemDetails);


		this.errorMessage = null;
		this.successMessage = null;
		this.showSaveBtn = true;


		let fields = ["allocatedQuantity"];
        for(let i = 0; i < fields.length; i++)
        {
            if (event != "") {
                fGroup.controls[fields[i]].setValidators([Validators.required, CustomValidator.requestedQuantity]);
                fGroup.controls[fields[i]].updateValueAndValidity();   
            }
            else {
                fGroup.controls[fields[i]].setValidators(null);
                fGroup.controls[fields[i]].updateValueAndValidity();  
            } 
        }
	}



}
