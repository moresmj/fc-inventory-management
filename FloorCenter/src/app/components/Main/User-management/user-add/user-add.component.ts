import { Component,Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { UserService } from '@services/user/user.service';
import { User } from '@models/user/user.model';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { ShowErrorsComponent } from '@components/show-errors/show-errors.component';
import { CustomValidator } from '@validators/custom.validator';


@Component({
	selector: 'app-user-add',
	templateUrl: './user-add.html'
})

export class UserAddComponent implements OnChanges {

	@Output() updatePage : EventEmitter<String> = new EventEmitter<String>();

	newUserForm : FormGroup;
	assignmentList : Dropdown[] = [];
	userTypeList : Dropdown[] = [];
	storeList : Dropdown[] = [];
	warehouseList : Dropdown[] = [];

	successMessage : string;	
	errorMessage : any;

	constructor(
		private fb : FormBuilder,
		private _userService : UserService,
		private _commonViewService : CommonViewService
		) 
	{
		this.createForm();
		this.load();
	}


	load(): void {

		this._commonViewService.getCommonList("assignments",true)
            					.subscribe(ddl => {this.assignmentList = ddl; });  

		this._commonViewService.getCommonList("usertypes",true)
            					.subscribe(ddl => {this.userTypeList = ddl; });  

		this._commonViewService.getCommonList("stores")
            					.subscribe(ddl => {this.storeList = ddl; }); 		

		this._commonViewService.getCommonList("warehouses")
            					.subscribe(ddl => {this.warehouseList = ddl; });  

	}

	createForm() {
		this.newUserForm = this.fb.group({
			UserName : new FormControl('',Validators.required),
			FullName : new FormControl('',Validators.required),
		    Password : new FormControl('', Validators.compose([Validators.required,Validators.minLength(8)])),
			Assignment : new FormControl('',Validators.required),
			StoreId : new FormControl(''),
			WarehouseId : new FormControl(''),
			UserType : new FormControl('',Validators.required)
		});
	}


	onSubmit(data : any) {
		let formData = this.newUserForm.value;

		if (formData["Assignment"] === 6) {
				formData["WarehouseId"] = null;
				formData["StoreId"] = null;
		}
		else if (formData["Assignment"] === 7) {
			formData["StoreId"] = null;
		}
		else if (formData["Assignment"] === 8) {
			formData["WarehouseId"] = null;
		}
		
	    this._userService.newRecord(formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Added";
            this.errorMessage = null; 
            this.newUserForm.reset();       
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}

	ngOnChanges(changes : SimpleChanges) {
		this.errorMessage = null;
		this.successMessage = null;
	}

}