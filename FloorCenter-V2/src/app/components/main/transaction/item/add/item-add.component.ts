import { Component, OnInit ,Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';

import { Ng2ImgMaxService } from 'ng2-img-max';
import { RequestService } from '@services/request.service';
import 'rxjs/add/operator/delay';

import { AppConstants } from '@common/app-constants/app-constants';


@Component({
  selector: 'app-item-add',
  templateUrl: './item-add.html'
})

export class ItemAddComponent implements OnInit, OnChanges {

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

    module : string = "item-test";

  constructor(
      private fb : FormBuilder,
      private _itemService : ItemService,
      private _commonViewService : CommonViewService,
      private _requestService : RequestService,
      private ng2ImgMax: Ng2ImgMaxService
    ) 
  {
    this.createForm();
    this.load();
    //this.changes();
    this._requestService.action = "items";
  }

  ngOnInit() {
      this.newForm.valueChanges.subscribe(form => {
          this.successMessage = null;
          this.errorMessage = null;
    });
  }

  onFileChange(event) {
    if(event.target.files.length > 0) {
      let file = event.target.files[0];


      if (file.type.includes("image")) {
          if (file["size"] <= 7000000) {


              this.isUploading = true;
              this.imagePath = "assets/images/spinner.gif";
 
              this.ng2ImgMax.resizeImage(file, 800, 750).subscribe(
                result => {
         
                  this.uploadedImage = new File([result], result.name);

                  // If image > 1.2mb save resized image else save uploaded.
                  this.uploadedImage = (file["size"] >= 1300000) ? this.uploadedImage : file;

                  this._requestService
                      .uploadImage(this.uploadedImage)
                      .subscribe(res => {
                          this.imagePath = "assets/images/item-images/image_temp/" + file.name + "?_=" + Date.now();
                          this.isUploading = false;
                  });

                  this.newForm.get('ImageName').setValue(file.name); 
                  this.imageErrorMessage = null; 
                },
                error => {
                  console.log(error);
                }
              );

          }
          else {
              this.imageErrorMessage = "Image too large (Maximum Size of 7MB only)";
          }
      }
      else {
        this.imageErrorMessage = "Invalid File Format";
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
    let formData = this.newForm.value;
    this._requestService.action = "items";

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
      SerialNumber : new FormControl('',[Validators.required, CustomValidator.SerialNumber]),
      Code : new FormControl('',[Validators.required]),
      Name : new FormControl('',[Validators.required]),
      SRP : new FormControl('',[Validators.required, CustomValidator.SRP,Validators.maxLength(16)]),
      Tonality : new FormControl('',[Validators.required, CustomValidator.shouldNotContainDot]),
      SizeId : new FormControl('',Validators.required),
      ImageName : new FormControl(''),
      Description : new FormControl(''),
      Remarks : new FormControl(''),
      QtyPerBox: new FormControl('1', [Validators.required, CustomValidator.shouldNotContainDot,CustomValidator.requestedQuantity]),
      BoxPerPallet: new FormControl('1',[Validators.required, CustomValidator.shouldNotContainDot,CustomValidator.requestedQuantity]),
      Cost: new FormControl('',[Validators.required, CustomValidator.Cost,Validators.maxLength(16)]),
    });
  }

  createSizeForm() {
    this.newSizeForm = this.fb.group({
      Name : new FormControl('',Validators.required),
      
    });
  }


  ngOnChanges(changes : SimpleChanges) {
    this.errorMessage = null;
    this.successMessage = null;
  }



}
