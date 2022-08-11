import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { UserService } from '@services/user/user.service';
import { User } from '@models/user/user.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

declare var $jquery: any;
declare var $:any;

@Component({
	selector: 'app-user-add',
	templateUrl: './user-add.html'
})

export class UserAddComponent implements OnChanges {

	@Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
	@Output() updatePage : EventEmitter<String> = new EventEmitter<String>();

	newUserForm : FormGroup;
	assignmentList : Dropdown[] = [];
	assignmentFullList : any = [];
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

	ngOnInit() {
	  	this.newUserForm.valueChanges.subscribe(form => {
	    	this.successMessage = null;
	    	this.errorMessage = null;
		});
	}

	load(): void {

		this._commonViewService.getCommonList("assignments",true)
								.subscribe(ddl => {this.assignmentList = ddl;
													this.assignmentFullList = ddl; });  

		this._commonViewService.getCommonList("usertypes",true)
            					.subscribe(ddl => {
									
									this.userTypeList = ddl; 
									
								});  

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
			UserType : new FormControl('',Validators.required),
			emailAddress : new FormControl('',[Validators.required, CustomValidator.emailAddress]),
			ContactNumber : new FormControl(''),
			Assignment : new FormControl('',Validators.required),
			StoreId : new FormControl(''),
			WarehouseId : new FormControl('')
		});
	}

	setAssignment(value : number) {

		let ass = 
			{ 
				2 : "WarehouseId",
				3 : "StoreId" 
			};

			for(let key in ass){
				if (key === value.toString()) {
					this.newUserForm.controls[ass[key]].setValidators([Validators.required]);
            		this.newUserForm.controls[ass[key]].updateValueAndValidity();	
				}
				else
				{
					this.newUserForm.controls[ass[key]].setValidators(null);
            		this.newUserForm.controls[ass[key]].updateValueAndValidity();	
				}
			}
	}


	onSubmit(data : any) {
		let formData = this.newUserForm.value;
		console.log(formData);

		if (formData["Assignment"] === 1) {
				formData["WarehouseId"] = null;
				formData["StoreId"] = null;
		}
		else if (formData["Assignment"] === 3) {
			formData["StoreId"] = null;
		}
		else if (formData["Assignment"] === 2) {
			formData["WarehouseId"] = null;
		}
		
	    this._userService.newRecord(formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            this.newUserForm.reset(); 
            
            this.successMessage = "Record Succesfully Added";
            this.errorMessage = null;       
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
	}

	onSelectRole(role : any){
		if(role == 1 || role == 2 || role == 5)
		{
			this.assignmentList = this.assignmentFullList;
			//set value for assignment Main office
			this.newUserForm.controls.Assignment.setValue(1);
			this.setAssignment(1);
			$("#assignment").val(1);
			
		}
		else{
			this.assignmentList = this.assignmentFullList;
			this.newUserForm.controls.Assignment.setValue("");
			$("#assignment").val('');
			
			if(role == 3 || role == 4)
				{
					this.assignmentList = this.assignmentList.filter(p => p["value"] != 1 && p["value"] != 5);
				}
			
		}
		
		console.log(role);
		console.log(this.newUserForm.value)

		
		
	}

	cancelAdd(){
		this.hidePanelAdd.emit();
		this.successMessage = null;
		this.errorMessage = null;
	}

	ngOnChanges(changes : SimpleChanges) {
		this.errorMessage = null;
		this.successMessage = null;
	}

}