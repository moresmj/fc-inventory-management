import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges
} from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  FormControl,
  Validators
} from "@angular/forms";

import { UserService } from "@services/user/user.service";
import{ RequestService } from "@services/request.service";

import { CustomValidator } from "@validators/custom.validator";
import { CommonViewService } from "@services/common/common-view.service";
import { Dropdown } from "@models/common/dropdown.model";

declare var $jquery: any;
declare var $: any;

@Component({
  selector: "app-user-dealer-add",
  templateUrl: "./user-dealer-add.html"
})
export class UserDealerAddComponent implements OnInit, OnChanges {
  @Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

  newUserForm: FormGroup;
  storeList: Dropdown[] = [];
  storesHandled: any;

  dropdownSettings = {};

  successMessage: string;
  errorMessage: any;

  constructor(
    private fb: FormBuilder,
    private _userService: UserService,
    private _commonViewService: CommonViewService,
    private _requestService : RequestService,
  ) {
    this._requestService.action = "users/dealer"
    this.createForm();
    this._commonViewService.getCommonList("stores").subscribe(ddl => {
      this.storeList = ddl;
    });
  }

  ngOnInit() {
    this.newUserForm.valueChanges.subscribe(form => {
      this.successMessage = null;
      this.errorMessage = null;
    });

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


  createForm() {
    this.newUserForm = this.fb.group({
      UserName: new FormControl("", Validators.required),
      FullName: new FormControl("", Validators.required),
      Password: new FormControl(
        "",
        Validators.compose([Validators.required, Validators.minLength(8)])
      ),
      emailAddress: new FormControl("", [
        Validators.required,
        CustomValidator.emailAddress
      ]),
      ContactNumber: new FormControl("")
    });
  }

  onSubmit(data: any) {
    let formData = this.newUserForm.value;
    formData["storesHandled"] = this.storesHandled.map(x => x["id"]);
    console.log(formData);

    this._requestService.newRecord(formData).subscribe(
      successCode => {
        this.updatePage.emit("loadPageRecord");
        this.newUserForm.reset();
        this.storesHandled = null;

        this.successMessage = "Record Succesfully Added";
        this.errorMessage = null;
      },
      error => {
        this.errorMessage = this._commonViewService.getErrors(error);
        this.successMessage = null;
      }
    );
  }

  cancelAdd() {
    this.hidePanelAdd.emit();
    this.successMessage = null;
    this.errorMessage = null;
  }

  ngOnChanges(changes: SimpleChanges) {
    this.errorMessage = null;
    this.successMessage = null;
  }
}
