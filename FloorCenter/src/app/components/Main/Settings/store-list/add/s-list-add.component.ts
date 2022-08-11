import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import { StoreService } from '@services/store/store.service';
import { Store } from '@models/store/store.model';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { ShowErrorsComponent } from '@components/show-errors/show-errors.component';


@Component({
	selector: 'app-s-list-add',
	templateUrl: './s-list-add.html'
})

export class StoreListAddComponent implements OnChanges  {

	@Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	newStoreForm : FormGroup;

	companyList : Dropdown[] = [];
	warehouseList : Dropdown[] = [];

	successMessage : string;
	errorMessage : any;

	constructor(
		private fb : FormBuilder,
		private _storeService : StoreService,
		private _commonViewService : CommonViewService) {

		this.createForm();
		this.loadDropdown();
	}


	private loadDropdown(): void {
    
    	this._commonViewService.getCommonList("companies")
                     			.subscribe(ddl => {this.companyList = ddl; });  
        this._commonViewService.getCommonList("warehouses")
                      	 		.subscribe(ddl => {this.warehouseList = ddl; });   
 	} 


	onSubmit() {
	     let store = this.newStoreForm.value;

        this._storeService.newRecord(store)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            this.successMessage = "Record Succesfully Added";
            this.errorMessage = null;
            this.reset();

        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.successMessage = null;
        });
	}


	cancelAdd(){
		this.hidePanelAdd.emit();
	}

	reset(){
        this.createForm();
    }

    createForm() {
		this.newStoreForm = this.fb.group({
			Code : new FormControl('',Validators.required),
			Name : new FormControl('',Validators.required),
			CompanyId : new FormControl('',Validators.required),
			Address : new FormControl(''),
			ContactNumber : new FormControl(''),
			WarehouseId : new FormControl('',Validators.required)
		});
	}

	ngOnChanges(changes : SimpleChanges)
	{
		this.successMessage = null;
		this.errorMessage = null;
	}

}

