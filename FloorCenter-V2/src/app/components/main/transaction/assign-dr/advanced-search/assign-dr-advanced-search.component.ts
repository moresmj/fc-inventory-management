import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

@Component({
	selector: "app-assign-dr-advanced-search",
	templateUrl : "./assign-dr-advanced-search.html"
})

export class AssignDrAdvancedSearchComponent {

	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;
	
	searchForm : FormGroup;
	requestStatus : any  = [];
	storeList : Dropdown[] = [];
	warehouseList : Dropdown[] = [];
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	constructor(private fb : FormBuilder,
		private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	createForm() {
		this.searchForm = this.fb.group({
			StoreId : new FormControl(''),
			WarehouseId : new FormControl(''),
			transactionNo : new FormControl(''),
			PONumber : new FormControl(''),
			PODateFrom : new FormControl(''),
			PODateTo : new FormControl(''),
			ItemName : new FormControl(''),
			WithDRNumber : new FormControl(false),
		});
	}

	 /* chkEvent(event)
	{
		let value = event.currentTarget.value;

		if (event.target.checked) {
			if (this.requestStatus.indexOf(value) == -1) {
				this.requestStatus.push(value);
			}	
		}
		else {

			let index = this.requestStatus.indexOf(value)

			if (index != -1) {
				this.requestStatus.splice(index, 1);
			}
		}
	}*/
	load() {

	    	this._commonViewService.getCommonList("stores")
	                     			.subscribe(ddl => { this.storeList = ddl; });
	        this._commonViewService.getCommonList("warehouses")
	                     			.subscribe(ddl => { this.warehouseList = ddl; });    



		}
	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );

		let formData = Object.assign([] , this.searchForm.value ); 
		//formData["requestStatus"] = this.requestStatus;


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
		formData.WithDRNumber = false;
		this.updatePage.emit(formData);
	}



}