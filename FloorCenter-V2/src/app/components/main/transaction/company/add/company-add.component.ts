import { Component, OnInit ,Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';


import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';


import { RequestService } from '@services/request.service';
import 'rxjs/add/operator/delay';

import { MainComponentBase } from '@main/main-componentbase';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
  selector: 'app-company-add',
  templateUrl: './company-add.html'
})

export class CompanyAddComponent extends MainComponentBase implements OnInit, OnChanges {

  @Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() updatePage : EventEmitter<String> = new EventEmitter<String>();
  @ViewChild("fileInput") fileInput;

  newForm : FormGroup;
  newSizeForm : FormGroup;
  sizeList : any = [];
  imagePath : string = null;
  isUploading : boolean = false;

  newSize : any;


    imageErrorMessage : string;
    successMessage : string;  
    errorMessage : any;


    uploadedImage: File;

    module : string = "company";

  constructor(
  
      private fb : FormBuilder,
      private _companyService : ApiBaseService,
      private _commonViewService : CommonViewService,
      private _requestService : RequestService,
      private _spinnerService : Ng4LoadingSpinnerService

    ) 
  {
    super(_companyService,_commonViewService,_spinnerService);
    this.createForm();
    _requestService.action = "Companies";

  }

  ngOnInit() {
      this.newForm.valueChanges.subscribe(form => {
          this.successMessage = null;
          this.errorMessage = null;
    });
  }

  onSubmit() {
    let formData = this.newForm.value;

    this._requestService.newRecord(formData)
      .subscribe(successCode =>{
          this.newForm.reset();     
          this.updatePage.emit("loadPageRecord");

          this.successMessage = "Record Succesfully Added";
          this.errorMessage = null;   
      },
      error =>{
           this.errorMessage = this._commonViewService.getErrors(error);
           this.successMessage = null;
      });
  }

  cancelAdd(){
    this.hidePanelAdd.emit();
  }

  reset() {
      this.createForm();
  }

  createForm() {
    this.newForm = this.fb.group({
      Code : new FormControl('',Validators.required),
      Name : new FormControl('',Validators.required),
    });
  }


  ngOnChanges(changes : SimpleChanges) {
    this.errorMessage = null;
    this.successMessage = null;
  }



}
