import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges
} from "@angular/core";
import { FormGroup, FormBuilder } from "@angular/forms";

import { PageModuleService } from "@services/common/pageModule.service";
import { UserService } from "@services/user/user.service";
import { CommonViewService } from "@services/common/common-view.service";
import { Dropdown } from "@models/common/Dropdown.model";

@Component({
  selector: "app-user-dealer-update",
  templateUrl: "./user-dealer-update.html"
})
export class UserDealerUpdateComponent implements OnInit, OnChanges {
  userType: number;
  @Input() id: number;
  @Input() username: string;
  @Input() updateUserForm: FormGroup;
  @Input() storesHandled: any;
  @Input() isActive: any;

  @Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

  storeList: Dropdown[] = [];
  dropdownSettings = {};

  successMessage: string;
  errorMessage: any;

  constructor(
    private fb: FormBuilder,
    private _userService: UserService,
    private _commonViewService: CommonViewService,
    private pageModuleService: PageModuleService
  ) {
    this._commonViewService.getCommonList("stores").subscribe(ddl => {
      this.storeList = ddl;
    });
  }

  ngOnInit() {
    this.userType = parseInt(this.pageModuleService.userType);

    this.dropdownSettings = {
      singleSelection: false,
      idField: "id",
      textField: "name",
      selectAllText: "Select All",
      unSelectAllText: "UnSelect All",
      itemsShowLimit: 10,
      allowSearchFilter: true
    };
  }

  changeUserStatus(data: any) {
    this.updateUserForm.controls.isActive.setValue(data);
  }

  onSubmit(data: any) {
    let formData = this.updateUserForm.value;
    formData["storesHandled"] = this.storesHandled.map(x => x["id"]);

    this._userService.updateRecord(this.id, formData).subscribe(
      successCode => {
        this.updatePage.emit("loadPageRecord");

        // Reset Password and Confirm Password after success;
        formData["Password"] = null;
        this.updateUserForm.reset(formData);
        this.successMessage = "Record Succesfully Updated";
        this.errorMessage = null;
      },
      error => {
        this.errorMessage = this._commonViewService.getErrors(error);
        this.successMessage = null;
      }
    );
  }

  ngOnChanges(changes: SimpleChanges) {
    this.errorMessage = null;
    this.successMessage = null;
  }
}
