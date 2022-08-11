import { Component, ViewChild } from "@angular/core";
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
  NgModel
} from "@angular/forms";
import { DatePipe } from "@angular/common";

import { User } from "@models/user/user.model";

import{ RequestService } from "@services/request.service";

import { PageModuleService } from "@services/common/pageModule.service";
import { CustomValidator } from "@validators/custom.validator";
import { PagerComponent } from "@common/pager/pager.component";
import "rxjs/add/operator/map";

import { Angular2Csv } from "angular2-csv/Angular2-csv";

@Component({
  selector: "app-user-dealer-list",
  templateUrl: "./user-dealer-list.html"
})
export class UserDealerListComponent {
  userType: number;
  search: string;
  module: string = "user-dealer";

  allRecords: User[] = [];
  userList: User[] = [];
  totalRecordMessage: string;
  pageRecordMessage: string;

  updateForm: FormGroup;
  showAddPanel: boolean = false;

  selectedId: number;
  selectedUsername: string;
  storesHandled: any;
  isActive: any;

  displayMessage: any;
  public now: Date = new Date();
  pipe = new DatePipe("en-US");

  successMessage : any;

  constructor(
    private fb: FormBuilder,
    private pageModuleService: PageModuleService,
    private _requestService : RequestService,
  ) {}

  ngOnInit() {
    this.userType = parseInt(this.pageModuleService.userType);
  }

  @ViewChild(PagerComponent)
  private pager: PagerComponent;

  getUsers(pagerModel: any) {
    this.allRecords = pagerModel["allRecords"];
    this.userList = pagerModel["pageRecord"];
    this.totalRecordMessage = pagerModel["totalRecordMessage"];
    this.pageRecordMessage = pagerModel["pageRecordMessage"];
  }

  addPanelShow(): void {
    this.showAddPanel = !this.showAddPanel;
    this.successMessage = null;
  }

  onSearch() {
    this.successMessage = null;
    if (this.showAddPanel) {
      this.showAddPanel = false;
    }
  }

  reloadRecord(event: string) {
    if (this.pager[event]) {
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

  onBtnUpdateClick(data: any) {
    this.selectedId = data.id;
    this.selectedUsername = data.userName;
    this.storesHandled = data.handled;
    this.successMessage = null;

    console.log(this.isActive);
    this.updateForm = this.fb.group({
      FullName: new FormControl(data.fullName, Validators.required),
      Password: new FormControl(
        "",
        Validators.compose([Validators.required, Validators.minLength(8)])
      ),
      emailAddress: new FormControl(data.emailAddress, [
        Validators.required,
        CustomValidator.emailAddress
      ]),
      ContactNumber: new FormControl(data.contactNumber),
    });
  }

  onBtnUpdateStatus(data : any)
	{
		
    this._requestService.action = "Users/updatestatus"
    this.successMessage = null;

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

  toModel(detail: any): User {
    let model = new User({
      Id: detail.id,
      UserName: detail.userName,
      FullName: detail.fullName,
      EmailAddress: detail.emailAddress,
      LastLogin:
        detail.lastLogin != null
          ? new Date(detail.lastLogin).toLocaleString()
          : "",
    });

    return model;
  }

  downloadUserList() {
    var options = {
      fieldSeparator: ",",
      quoteStrings: '"',
      decimalseparator: ".",
      showLabels: true,
      headers: [
        "ID",
        "User Name",
        "Full Name",
        "Email Address",
        "Last Login",
      ]
    };
    let title = this.now;
    let record = this.allRecords.map(r => this.toModel(r));

    new Angular2Csv(record, title.toISOString(), options);
  }
}
