import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';
import { DatePipe } from '@angular/common';

import { User } from '@models/user/user.model';

import { PageModuleService } from '@services/common/pageModule.service'
import { CustomValidator } from '@validators/custom.validator';
import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';


import { RequestService } from '@services/request.service';


import { Angular2Csv } from 'angular2-csv/Angular2-csv';


@Component({
  selector: 'app-user-list',
	templateUrl: './user-list.html'
})

export class UserListComponent {


  	userType : number;
	search : string;
	module : string = "user";

 	allRecords : User[] = [];
	userList : User[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	updateForm : FormGroup;
  	showAddPanel : boolean  = false;

	selectedId : number;
	selectedUsername : string;
	isActive : any;

	displayMessage : any;
	successMessage : any;
	public now: Date = new Date();
	pipe = new DatePipe('en-US');

	constructor(
			private fb: FormBuilder,
			private pageModuleService: PageModuleService,
			private _requestService : RequestService) {

	}

	ngOnInit() {

    	this.userType = parseInt(this.pageModuleService.userType);

	}

	@ViewChild(PagerComponent)
	private pager : PagerComponent;

	getUsers(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
        this.userList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
	}

	addPanelShow(): void {
		this.showAddPanel = !this.showAddPanel;
		this.successMessage = null;
	}

	onSearch()
	{
	    if (this.showAddPanel) {
	    	this.showAddPanel = false;
		}
		this.successMessage = null;

	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecord() {

	    if (this.search == "" && this.userList.length == 0) {
	      this.pager["loadPageRecord"](1);
	    }
	    else{
	      this.pager["filterPageRecord"](this.search);
	    }
	       
	}

	onBtnUpdateStatus(data : any)
	{
		
		this._requestService.action = "Users/updatestatus"

		this._requestService.updateRecord(this.selectedId,data)
        .subscribe(successCode =>{
		  console.log("updated");
		  this.reloadRecord("loadPageRecord");
		  this.successMessage = "Record successfully Updated";
		  $("#saveModal").modal("hide");
		  
        },
        error =>{

			console.log(error);

        });
	}

	onBtnUpdateClick(data : any) {

		this.successMessage = null;
		this.selectedId = data.id;
		this.selectedUsername = data.userName;
		this.isActive = data.isActive === null  ? false : data.isActive;
		console.log(this.isActive);
		this.updateForm = this.fb.group({
			FullName : new FormControl(data.fullName,Validators.required),
		    Password : new FormControl('', Validators.compose([Validators.required,Validators.minLength(8)])),
		    UserType : new FormControl(data.userType,Validators.required),
		    emailAddress : new FormControl(data.emailAddress,[Validators.required, CustomValidator.emailAddress]),
			ContactNumber : new FormControl(data.contactNumber),
			Assignment : new FormControl(data.assignment,Validators.required),
			StoreId : new FormControl(data.storeId,(data.storeId != null) ? Validators.required : null),
			WarehouseId : new FormControl(data.warehouseId,(data.warehouseId != null) ? Validators.required : null),
			isActive : new FormControl(data.isActive)
		});
	}

	toModel(detail : any): User {
	    let model = new User({
	      Id : detail.id,
	      UserName : detail.userName,
	      FullName : detail.fullName,
	      EmailAddress : detail.emailAddress,
	      AssignmentStr : detail.assignmentStr,
	      LastLogin : 	(detail.lastLogin != null) ? new Date(detail.lastLogin).toLocaleString() : '' ,
	      UserTypeStr : detail.userTypeStr
	    });

	    return model;
	}


	downloadUserList(){
		
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','User Name','Full Name', "Email Address", "Assigned To" , "Last Login", "Level"]				
		};
		let title = this.now;
		let record = this.allRecords.map(r => this.toModel(r));

		new Angular2Csv(record, title.toISOString(), options);
	}

}