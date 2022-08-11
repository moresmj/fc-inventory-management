import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

@Component({
	selector: "app-receive-return-items-advanced-search",
	templateUrl : "./receive-return-items-advanced-search.html"
})

export class ReceiveReturnItemsAdvancedSearchComponent {
	

	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;

	searchForm : FormGroup;
	transactionTypeList : any  = [];
	deliveryStatusList : any = [];
	storeList : any = [];
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	constructor(
		private fb : FormBuilder,
		private _commonViewService : CommonViewService) {

		this.createForm();
		this.loadDropdown();
	}

	createForm() {
		this.searchForm = this.fb.group({
			TransactionNo : new FormControl(''),
			ReturnFormNumber : new FormControl(''),
			DRNumber : new FormControl(''),
			StoreId : new FormControl(''),
			DeliveryStatus : new FormControl('')

		});
	}

	private loadDropdown(): void {
    
    	this._commonViewService.getCommonList("transactiontypes",true)
								 .subscribe(ddl => { this.transactionTypeList = ddl; });  
		
		this._commonViewService.getCommonList("deliverystatus",true)
								 .subscribe(ddl => { this.deliveryStatusList = ddl.filter(p => p["value"] == 1 || p["value"] == 3); });

						
								 
		this._commonViewService.getCommonList("Stores")
								 .subscribe(ddl => { this.storeList = ddl; });				

 	} 

	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );

		let formData = Object.assign([] , this.searchForm.value );

		$("#advanceSearch").modal("hide");

		console.log(formData);
		this.updatePage.emit(formData);
	}

	resetCriteriaFromSearch(formData : any) {
		for(let key in formData) {
			this.searchForm.get(key).setValue(formData[key]);
		}		
	}


	onCancel() {
		if(!this.isFiltered) 
		{
			this.createForm();
		}
		else
		{
			this.resetCriteriaFromSearch(this.savedSearchForm.value);
		}
	}	

	onClear() {
		this.createForm();
		this.isFiltered = false;

		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}



}