import { Component, OnInit ,Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { RequestService } from '@services/request.service';
import 'rxjs/add/operator/delay';


@Component({
  selector: 'app-item-add2',
  templateUrl: './item-add.html'
})

export class ItemAddComponent implements OnInit, OnChanges {

  successMessageItem : string;
  
  @Output() successMessageItem2: EventEmitter<String> = new EventEmitter<String>();
  @Output() updatePage : EventEmitter<any> = new EventEmitter<any>();
  @Output() itemSerial : EventEmitter<String> = new EventEmitter<String>();
  @Input() Update : any;
  @ViewChild("fileInput") fileInput;

  newForm : FormGroup;
  newSizeForm : FormGroup;
  sizeList : Dropdown[] = [];
  imagePath : string = null;
  isUploading : boolean = false;


  imageerrorMessageItem : string;

  errorMessageItem : any;


    module : string = "item-test";

  constructor(
      private fb : FormBuilder,
      private _itemService : ItemService,
      private _commonViewService : CommonViewService,
      private _requestService : RequestService
    ) 
  {
    this.createForm();
    this.load();
    //this.changes();
    this._requestService.action = "items";
  }

  ngOnInit() {
      this.newForm.valueChanges.subscribe(form => {
          this.successMessageItem2 = null;
          this.errorMessageItem = null;
    });
  }

  onFileChange(event) {
    if(event.target.files.length > 0) {
      let file = event.target.files[0];


      if (file.type.includes("image")) {
          if (file["size"] <= 7000000) {


              this.isUploading = true;
              this.imagePath = "assets/images/spinner.gif";

              this._requestService
                  .uploadImage(file)
                  .subscribe(res => {
                      this.imagePath = "assets/images/item-images/image_temp/" + file.name + "?_=" + Date.now();
                      this.isUploading = false;
                  });

                this.newForm.get('ImageName').setValue(file.name); 
                this.imageerrorMessageItem = null; 
          }
          else {
              this.imageerrorMessageItem = "Image too large (Maximum Size of 7MB only)";
          }
      }
      else {
        this.imageerrorMessageItem = "Invalid File Format";
      }
    }
    else {
      this.newForm.get('ImageName').setValue(''); 
    }
  }


  load(): void {

    this._commonViewService.getCommonList("sizes")
                      .subscribe(ddl => {this.sizeList = ddl; });  



  }

  onAddSize(){
    this.createSizeForm();
  }

  onSubmit() {
    this._requestService.action = "items";
    let formData = this.newForm.value;

    this._requestService.newRecord(formData)
      .subscribe(successCode =>{
          this.newForm.reset();
          let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Record Succesfully Added"};
     
          this.updatePage.emit(emitValue);
          this.itemSerial.emit(formData.SerialNumber);

          this.errorMessageItem = null;  
          $("#details_modal").modal("hide"); 
          this.Update = null;
      },
      error =>{
           this.errorMessageItem = this._commonViewService.getErrors(error);
           this.successMessageItem2 = null;
      });
  }



  reset() {
      this.createForm();
  }

  createForm() {
    this.newForm = this.fb.group({
      SerialNumber : new FormControl('',[Validators.required, CustomValidator.SerialNumber]),
      Code : new FormControl('',[Validators.required, Validators.maxLength(50)]),
      Name : new FormControl('',[Validators.required, Validators.maxLength(255)]),
      SRP : new FormControl('',[Validators.required, CustomValidator.SRP,Validators.maxLength(16)]),
      Tonality : new FormControl('',[Validators.required, Validators.maxLength(50)]),
      SizeId : new FormControl('',Validators.required),
      ImageName : new FormControl(''),
      Description : new FormControl(''),
      QtyPerBox: new FormControl('1', [Validators.required, CustomValidator.shouldNotContainDot,CustomValidator.requestedQuantity]),
      BoxPerPallet: new FormControl('1',[Validators.required, CustomValidator.shouldNotContainDot,CustomValidator.requestedQuantity]),
      Cost: new FormControl('',[Validators.required, CustomValidator.Cost,Validators.maxLength(16)]),
      Remarks : new FormControl('')
    });
  }

    createSizeForm() {
    this.newSizeForm = this.fb.group({
      Name : new FormControl('',Validators.required),
      
    });
  }

  ngOnChanges(changes : SimpleChanges) {
    this.errorMessageItem = null;
    this.successMessageItem2 = null;
  }



}