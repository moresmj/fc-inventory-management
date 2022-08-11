import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';


@Component({
	selector: "app-s-delivery-advanced-search",
	templateUrl : "./s-delivery-advanced-search.html"
})

export class SalesDeliveryAdvancedSearchComponent {

	// Flag for list if filtered.
	isFiltered : boolean = true;
	// 
	savedSearchForm : FormGroup;
	savedDeliveryStatus : any = [];

	searchForm : FormGroup;

	deliveryStatus : any  = ["2"];
	deliveryStatusList : any = [];

	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	createForm() {
		this.searchForm = this.fb.group({
			TransactionNo : new FormControl(''),
			DRNumber : new FormControl(''),
			DeliveryDateFrom : new FormControl(''),
			DeliveryDateTo : new FormControl(''),
		});
	}

	private load(): void {

		this._commonViewService.getCommonList("deliverystatus",true)
		                  .subscribe(ddl => { 
		                    this.deliveryStatusList = ddl; 
		                    this.deliveryStatusList = this.deliveryStatusList.filter(x => x["value"] != 4);
		                  });  

	}

	chkEvent(event)
	{
		let value = event.currentTarget.value;

		if (event.target.checked) {
			if (this.deliveryStatus.indexOf(value) == -1) {
				this.deliveryStatus.push(value);
			}	
		}
		else {

			let index = this.deliveryStatus.indexOf(value)

			if (index != -1) {
				this.deliveryStatus.splice(index, 1);
			}
		}
	}

	resetCriteriaFromSearch(formData : any) {
		for(let key in formData) {
			this.searchForm.get(key).setValue(formData[key]);
		}		
	}


	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );
		this.savedDeliveryStatus = Object.assign([] , this.deliveryStatus ); 

		let formData = Object.assign([] , this.searchForm.value );  ;
		formData["deliveryStatus"] = this.deliveryStatus;

		$("#advanceSearch").modal("hide");
		this.updatePage.emit(formData);
	}

	onCancel() {
		if(!this.isFiltered) {
			this.createForm();
			this.deliveryStatus = [];

			// Uncheck checkboxes
			$("input[name='chkDeliveryStatus']").prop("checked",false);
		}
		else
		{
			if (this.savedSearchForm != undefined) {
				this.resetCriteriaFromSearch(this.savedSearchForm.value);
			}
			$("input[name='chkDeliveryStatus']").prop("checked",false);
			for(let i = 0; i < this.savedDeliveryStatus.length; i++) {
				switch(this.savedDeliveryStatus[i]) { 
				    case "1": { 
				      	$("#chkDeliveryStatus0").prop("checked",true);
						break; 
   					}
				    case "2": { 
				      	$("#chkDeliveryStatus1").prop("checked",true); 
						break; 
   					} 
				    case "3": { 
				      	$("#chkDeliveryStatus2").prop("checked",true);
						break; 
   					} 
				}
			}
		}
	}	

	onClear() {
		this.createForm();
		this.isFiltered = false;
		this.deliveryStatus = [];

		// Uncheck checkboxes
		$("input[name='chkDeliveryStatus']").prop("checked",false);

		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}



}