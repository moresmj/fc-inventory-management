import { Component, OnInit ,Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { Item } from '@models/item/item.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { RequestService } from '@services/request.service';
import 'rxjs/add/operator/delay';


@Component({
  selector: 'app-add-size',
  templateUrl: './add-size.html'
})

export class AddSizeComponent implements OnInit, OnChanges {

  @Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() updateSize : EventEmitter<String> = new EventEmitter<String>();
  @ViewChild("fileInput") fileInput;
  @Input()newSizeForm: FormGroup;

  newForm : FormGroup;
  sizeList : Dropdown[] = [];
  imagePath : string = null;
  isUploading : boolean = false;


  imageErrorMessage : string;
  successMessage : string;  
  errorMessage : any;


    module : string = "item-test";

  constructor(
      private fb : FormBuilder,
      private _baseService : ApiBaseService,
      private _commonViewService : CommonViewService,
      private _requestService : RequestService
    ) 
  {
    this.createForm();
    this._baseService.action = "sizes";
    this.successMessage = null;
    this.errorMessage = null;
  }

  ngOnInit() {
      this.newSizeForm.valueChanges.subscribe(form => {
          this.successMessage = null;
          this.errorMessage = null;
    });
  }





  onSubmit() {
    let formData = this.newSizeForm.value;

    this._baseService.newRecord(formData)
      .subscribe(successCode =>{
          this.newSizeForm.reset();     
          this.updateSize.emit("update");
   

          this.successMessage = "Record Succesfully Added";
          this.errorMessage = null;   
            setTimeout(function() {
                  $('#add_size').modal("hide");
              }, 1000);  
      },
      error =>{
           this.errorMessage = this._commonViewService.getErrors(error);
           this.successMessage = null;
      });
  }

  reset() {
      this.createForm();
  }

  createForm() {
    this.newSizeForm = this.fb.group({
      Name : new FormControl('',Validators.required),
      
    });
  }

  ngOnChanges(changes : SimpleChanges) {
    this.errorMessage = null;
    this.successMessage = null;
  }

  onChange(ch : any){
    this.successMessage = null;
    this.errorMessage = null;

  }



}
