import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

@Component({
	selector: "app-receive-items-advanced-search",
	templateUrl : "./receive-items-advanced-search.html"
})

export class ReceiveItemsAdvancedSearchComponent {
	

	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;

	searchForm : FormGroup;
	transactionTypeList : any  = [];
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	constructor(
		private fb : FormBuilder,
		private _commonViewService : CommonViewService) {

		this.createForm();
		this.loadDropdown();
	}

	createForm() {
		this.searchForm = this.fb.group({
			TransactionType : new FormControl(''),
			TransactionNo : new FormControl(''),
			PONumber : new FormControl(''),
			DRNumber : new FormControl(''),
			DeliveryDateFrom : new FormControl(''),
			DeliveryDateTo : new FormControl(''),
		});
	}

	private loadDropdown(): void {
    
    	this._commonViewService.getCommonList("transactiontypes",true)
                     			.subscribe(ddl => { this.transactionTypeList = ddl; });  

 	} 

	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );

		let formData = Object.assign([] , this.searchForm.value );

		$("#advanceSearch").modal("hide");
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