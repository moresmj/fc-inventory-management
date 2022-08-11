import { Component, OnInit, Input, Output,EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { NgForm, FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

import { PageModuleService } from '@services/common/pageModule.service';
import { UserService } from '@services/user/user.service';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/Dropdown.model';

@Component({
	selector: 'app-user-update',
	templateUrl: './user-update.html'
})

export class UserUpdateComponent implements OnInit, OnChanges {

  	userType : number;
	@Input() id : number;
	@Input() username : string;
	@Input() updateUserForm : FormGroup;
	@Input() isActive : any;


	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	assignmentList : Dropdown[] = [];
	userTypeList : Dropdown[] = [];
	assignmentFullList : any = [];
	storeList : Dropdown[] = [];
	warehouseList : Dropdown[] = [];

	successMessage : string;
	errorMessage : any;

	constructor(
		private fb : FormBuilder,
		private _userService : UserService,
		private _commonViewService : CommonViewService,
		private pageModuleService: PageModuleService
		) 
	{
		this.loadDropdown();
	}

	ngOnInit() {
    	this.userType = parseInt(this.pageModuleService.userType);
	}

	loadDropdown(): void {

		this._commonViewService.getCommonList("assignments",true)
								.subscribe(ddl => {this.assignmentList = ddl;
								this.assignmentFullList = ddl; });  

		this._commonViewService.getCommonList("usertypes",true)
            					.subscribe(ddl => {this.userTypeList = ddl; });  

		this._commonViewService.getCommonList("stores")
            					.subscribe(ddl => {this.storeList = ddl; }); 		

		this._commonViewService.getCommonList("warehouses")
            					.subscribe(ddl => {this.warehouseList = ddl; });  

	}

	setAssignment(value : number) {
		console.log(value);

		let ass = 
			{ 
				2 : "WarehouseId",
				3 : "StoreId" 
			};

			for(let key in ass){
				if (key === value.toString()) {
					this.updateUserForm.controls[ass[key]].setValidators([Validators.required]);
            		this.updateUserForm.controls[ass[key]].updateValueAndValidity();	
				}
				else
				{
					this.updateUserForm.controls[ass[key]].setValidators(null);
            		this.updateUserForm.controls[ass[key]].updateValueAndValidity();	
				}
			}
	}

	onSelectRole(role : any){
		if(role == 1 || role == 2 || role == 5)
		{
			this.assignmentList = this.assignmentFullList;
			//set value for assignment Main office
			this.updateUserForm.controls.Assignment.setValue(1);
			this.setAssignment(1);
			$("#assignment").val(1);
			
		}
		else{
			this.assignmentList = this.assignmentFullList;
			this.updateUserForm.controls.Assignment.setValue("");
			$("#assignment").val('');
			
			if(role == 3 || role == 4)
				{
					this.assignmentList = this.assignmentList.filter(p => p["value"] != 1 && p["value"] != 5);
				}
			
		}
		
		console.log(role);
		console.log(this.updateUserForm.value)

		
		
	}

	changeUserStatus(data : any)
	{
		this.updateUserForm.controls.isActive.setValue(data);
	}

	onClose()
	{
		$('#isActive').prop('checked', this.isActive);

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

		if($('#isActive:checked').val() === "on")
		{
			formData.isActive = true;
		}
		else
		{
			formData.isActive = false;
		}


	    this._userService.updateRecord(this.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            
            // Reset Password and Confirm Password after success;
            formData["Password"] = null;
			this.updateUserForm.reset(formData);
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
		$('#isActive').prop('checked', this.isActive);
	}


}