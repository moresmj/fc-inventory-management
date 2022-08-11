import { Component, Input, Output, EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

declare var jquery:any;
declare var $:any;


@Component({
	selector : 'app-im-incoming-advanced-search',
	templateUrl : './im-incoming-advanced-search.html'
})

export class InventoryMonitoringAdvancedSearchComponent {
	
	// Flag for list if filtered.
	isFiltered : boolean = true;
	// 
	savedSearchForm : FormGroup;

	searchForm : FormGroup;
	sizeList : Dropdown[] = [];

	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();



	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	load() {

    	this._commonViewService.getCommonList("sizes")
                     			.subscribe(ddl => { this.sizeList = ddl; });  

	}


	createForm() {
		this.searchForm = this.fb.group({
			serialNumber : new FormControl(''),
			code : new FormControl(''),
			itemName : new FormControl(''),
			sizeId : new FormControl(''),
			tonality : new FormControl('')
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

		let formData = Object.assign([] , this.searchForm.value );

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
		this.searchForm.reset();
		this.isFiltered = false;

		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}

}