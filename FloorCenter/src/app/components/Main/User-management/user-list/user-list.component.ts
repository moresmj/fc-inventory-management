import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { UserService } from '@services/user/user.service';
import { User } from '@models/user/user.model';

import { UserListFilter } from '@filters/user/user-list.filter';
import { PagerComponent } from '@pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { CustomValidator } from '@validators/custom.validator';

@Component({
	selector: 'app-user-list',
	templateUrl: './user-list.html'
})

export class UserListComponent {

	search : string;
	module : string = "user";

 	allRecords : User[] = [];
	userList : User[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	updateForm : FormGroup;

	selectedId : number;
	selectedUsername : string;

	displayMessage : any;
	public now: Date = new Date();

	constructor(private fb: FormBuilder) {}

	@ViewChild(PagerComponent)
	private pager : PagerComponent;

	getUsers(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
        this.userList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
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

	onBtnUpdateClick(data : any) {

		this.selectedId = data.id;
		this.selectedUsername = data.userName;
		this.updateForm = this.fb.group({
			FullName : new FormControl(data.fullName,Validators.required),
		    Password : new FormControl('', Validators.compose([Validators.required,Validators.minLength(8)])),
			Assignment : new FormControl(data.assignment,Validators.required),
			StoreId : new FormControl(data.storeId),
			WarehouseId : new FormControl(data.warehouseId),
			UserType : new FormControl(data.userType,Validators.required)
		});
	}

	toModel(detail : any): User {
	    let model = new User({
	      Id : detail.id,
	      UserName : detail.userName,
	      FullName : detail.fullName,
	      AssignmentStr : detail.assignmentStr,
	      LastLogin : 'Not Yet Specified',
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
			headers: ['ID','UserName','FullName', 'Assignment', 'Last Login','UserType']
				
		};
		let title = this.now;
		let record = this.allRecords.map(r => this.toModel(r));

		new Angular2Csv(record, title.toISOString(), options);
	}

}