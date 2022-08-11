import { Component, Input, Output, EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

declare var jquery:any;
declare var $:any;


@Component({
	selector: 'app-im-advanced-search',
	templateUrl: './im-advanced-search.html'
})

export class InventoryAdvancedSearchComponent {
	
	// Flag for list if filtered.
	isFiltered : boolean = true;
	// 
	savedSearchForm : FormGroup;
	@Input() searchForm : FormGroup;
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	constructor()
	{

	}

	resetCriteriaFromSearch(formData : any, isReset : boolean) {
		for(let key in formData) {

			let value = (isReset) ? false : formData[key];

			this.searchForm.controls[key].setValue(value);
            this.searchForm.controls[key].updateValueAndValidity();   
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
			this.searchForm.reset();
		}
		else
		{
			if (this.savedSearchForm != undefined) {
				this.resetCriteriaFromSearch(this.savedSearchForm.value, false);
			}
		}
	}	

	onClear() {

		this.resetCriteriaFromSearch(this.searchForm.value, true);
		this.isFiltered = false;

		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}

}