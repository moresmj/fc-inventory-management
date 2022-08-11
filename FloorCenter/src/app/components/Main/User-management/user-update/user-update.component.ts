import { Component, OnInit, Input, Output,EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { NgForm, FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

import { UserService } from '@services/user/user.service';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/Dropdown.model';

import { ShowErrorsComponent } from '@components/show-errors/show-errors.component';
import { CustomValidator } from '@validators/custom.validator';

@Component({
	selector: 'app-user-update',
	templateUrl: './user-update.html'
})

export class UserUpdateComponent implements OnInit, OnChanges {


	@Input() id : number;
	@Input() username : string;
	@Input() updateUserForm : FormGroup;


	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

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
		this.loadDropdown();
	}

	ngOnInit() {
		
	}

	loadDropdown(): void {

		this._commonViewService.getCommonList("assignments",true)
            					.subscribe(ddl => {this.assignmentList = ddl; });  

		this._commonViewService.getCommonList("usertypes",true)
            					.subscribe(ddl => {this.userTypeList = ddl; });  

		this._commonViewService.getCommonList("stores")
            					.subscribe(ddl => {this.storeList = ddl; }); 		

		this._commonViewService.getCommonList("warehouses")
            					.subscribe(ddl => {this.warehouseList = ddl; });  

	}

	onSubmit(data : any) {
		let formData = this.updateUserForm.value;

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


	    this._userService.updateRecord(this.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            
            // Reset Password and Confirm Password after success;
            this.updateUserForm.controls.Password.setValue("");
            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;
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