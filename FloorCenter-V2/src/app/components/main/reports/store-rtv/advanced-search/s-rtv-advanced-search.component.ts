import { Component, Input, Output, EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';

declare var jquery:any;
declare var $:any;


@Component({
	selector: 'app-s-rtv-advanced-search',
	templateUrl: './s-rtv-advanced-search.html'
})

export class StoreRTVAdvancedSearchComponent {
	
	// Flag for list if filtered.
	isFiltered : boolean = true;
	// 
	savedSearchForm : FormGroup;

	searchForm : FormGroup;
	storeList : any = [];


	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	load() {

    	this._commonViewService.getCommonList("stores")
                     			.subscribe(ddl => { this.storeList = ddl; });  

	}


	createForm() {
		this.searchForm = this.fb.group({
			storeId : new FormControl(''),
			code : new FormControl(''),
			returnFormNumber : new FormControl(''),
			drNumber : new FormControl(''),
			requestDateFrom : new FormControl(''),
			requestDateTo : new FormControl('')
		});
	}


	resetCriteriaFromSearch(formData : any) {
		for(let key in formData) {
			this.searchForm.get(key).setValue(formData[key]);
		}		
	}


	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );

		let formData = Object.assign([] , this.searchForm.value );  ;

		$("#advanceSearch").modal("hide");
		this.updatePage.emit(formData);
	}

	onCancel() {
		if(!this.isFiltered) {
			this.createForm();
		}
		else
		{
			if (this.savedSearchForm != undefined) {
				this.resetCriteriaFromSearch(this.savedSearchForm.value);
			}
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