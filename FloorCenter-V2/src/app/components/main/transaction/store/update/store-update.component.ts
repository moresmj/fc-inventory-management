import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import { PageModuleService } from '@services/common/pageModule.service';
import { StoreService } from '@services/store/store.service';
import { Store } from '@models/store/store.model';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';


@Component({
	selector: 'app-store-update',
	templateUrl: './store-update.html'
})


export class StoreUpdateComponent implements OnInit, OnChanges{

  	userType : number;
	@Input() id : number;
	@Input() code : string;
	@Input() updateStoreForm : FormGroup;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	successMessage : string;
	errorMessage : any;


	companyList : Dropdown[] = [];
	warehouseList : Dropdown[] = [];

	constructor(
		private fb : FormBuilder,
		private _storeService : StoreService,
		private _commonViewService : CommonViewService,
		private pageModuleService: PageModuleService) {
		
		this.loadDropdown();
		
	}

	ngOnInit() {
    	this.userType = parseInt(this.pageModuleService.userType);
	}


	private loadDropdown(): void {
    
    	this._commonViewService.getCommonList("companies")
                     			.subscribe(ddl => {this.companyList = ddl; });  
        this._commonViewService.getCommonList("warehouses")
                      	 		.subscribe(ddl => {this.warehouseList = ddl; });   
 	} 

	onSubmit() {
	    let formData = this.updateStoreForm.value;

        this._storeService.updateRecord(this.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;
        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.successMessage = null;
        });
	}

	ngOnChanges(changes : SimpleChanges)
	{
		this.successMessage = null;
		this.errorMessage = null;
	}

}