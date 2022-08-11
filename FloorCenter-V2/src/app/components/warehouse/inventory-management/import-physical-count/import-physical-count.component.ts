import { Component, OnInit ,Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup,FormArray, FormControl, Validators } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { ApiBaseService } from '@services/api-base.service';
import { RequestService } from '@services/request.service';
import 'rxjs/add/operator/delay';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { AppConstants } from '@components/common/app-constants/app-constants';

@Component({
  selector: 'app-import-physical-count',
  templateUrl: './import-physical-count.html'
})

export class ImportPhysicalCountComponent implements OnInit, OnChanges {

  @Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() updatePage : EventEmitter<String> = new EventEmitter<String>();
  @ViewChild("fileInput") fileInput;

  newForm : FormGroup;
  sizeList : Dropdown[] = [];
  imagePath : string = null;
  isUploading : boolean = false;


  imageErrorMessage : string;
  successMessage : string;  
  errorMessage : any;
  Details : any;


    module : string = "item-test";

  constructor(
      private fb : FormBuilder,
      private _itemService : ItemService,
      private _commonViewService : CommonViewService,
      private _apiBaseService : ApiBaseService,
      private spinnerService: Ng4LoadingSpinnerService,
      private _requestService : RequestService
    ) 
  {
    this.load();
    this._apiBaseService.action = "imports/physicalcount/warehouse";
    this._requestService.action = "imports/physicalcount/warehouse";
    this.createForm();
    //this.changes();
  }

  ngOnInit() {
      this.newForm.valueChanges.subscribe(form => {
          this.successMessage = null;
          this.errorMessage = null;
    });
  }

  onFileChange(event) {

    if(event.target.files.length > 0)
    {
      let file = event.target.files[0];

      if (file.name.includes(".csv")) {
            const reader = new FileReader();

            reader.onload = () => 
            {
                  let text = reader.result;
                  this.csvJSON(text);
                  

            };

          this.newForm.get('ImageName').setValue(file.name);
          this.imageErrorMessage = null; 
          reader.readAsText(event.target.files[0]);
        }
        else
        {
           this.imageErrorMessage = "Invalid File Format";
           this.newForm.get('ImageName').setValue("");

        }

    }
    else 
    {
      this.newForm.get('ImageName').setValue(''); 
      this.Details = null;
    }



    /*if(event.target.files.length > 0) {
      let file = event.target.files[0];


      if (file.type.includes("image")) {
          if (file["size"] <= 7000000) {


              this.isUploading = true;
              this.imagePath = "assets/images/spinner.gif";

              this._apiBaseService
                  .uploadImage(file)
                  .subscribe(res => {
                      this.imagePath = "assets/images/item-images/image_temp/" + file.name + "?_=" + Date.now();
                      this.isUploading = false;
                  });

                this.newForm.get('ImageName').setValue(file.name); 
                this.imageErrorMessage = null; 
          }
          else {
              this.imageErrorMessage = "Image too large (Maximum Size of 7MB only)";
          }
      }
      else {
        this.imageErrorMessage = "Invalid File Format";
      }
    }
*/
  }

  csvJSON(csv){
    var lines=csv.split("\n");
    var toDel = [];
    lines = lines.filter(entry => entry.trim() != '');
    if((lines.length - 1) > AppConstants.uploadMaxLength)
    {
      this.errorMessage = [AppConstants.uploadLengthErrorMessage];
    }


           for(let i = 0; i< lines.length;i++){

                
                  if(lines[i].length == 1 || lines[i].length == 0)
                  {
                     toDel.push(i);
                  }   
           }

            toDel.map(del => {
                  lines.splice(del);
            })


          var result = [];
          var errors = [];
          var regexNumber = new RegExp('^[0-9]+$');

          var headers=lines[0].split(",");

          for(var i=1;i<lines.length;i++){

              var obj = {};
              var currentline=lines[i].split(",");

              for(var j=0;j<headers.length;j++){
                if(j === 6)
                {
                  currentline[j] = currentline[j].replace(/(?:\\[rn]|[\r\n]+)+/g, "");
                  if (!regexNumber.test(currentline[j].toString())) {
                    errors.push("Invalid Value for Physical count on line #"+ i);
                  }                  
                }
                // Will not convert to int if remarks
                obj[headers[j].replace(/(?:\\[rn]|[\r\n]+)+/g, "")] = j !== 7 ? parseInt(currentline[j]) : currentline[j];  
              }
                
              result.push(obj);

            }
            this.spinnerService.hide();
            if(errors.length > 0)
            {
                  this.errorMessage = errors;
                  return;
            }

    this.Details = result;
    return result; //JSON
  }


  load(): void {

    this._commonViewService.getCommonList("sizes")
                      .subscribe(ddl => {this.sizeList = ddl; });  

  }

  onSubmit() {


    let details = { Details : this.Details}
    

    this._requestService.newRecord(details)
      .subscribe(successCode =>{
          this.newForm.reset();     
          this.updatePage.emit("loadPageRecord");

          this.successMessage = "Record Succesfully Added";
          this.errorMessage = null;
          this.Details = null; 
      },
      error =>{
           this.errorMessage = this._commonViewService.getErrors(error);
           this.successMessage = null;
           details = null;
           this.Details = null;
      });
  }

  cancelAdd(){
    this.hidePanelAdd.emit();
  }


  createForm() {

    this.newForm = this.fb.group({
      
      ImageName : new FormControl('')

    });
    
  }

  ngOnChanges(changes : SimpleChanges) {
    this.errorMessage = null;
    this.successMessage = null;
  }




}