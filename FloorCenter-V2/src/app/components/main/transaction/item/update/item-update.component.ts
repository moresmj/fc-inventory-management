import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { RequestService } from '@services/request.service';
import { PageModuleService } from '@services/common/pageModule.service';

import { Ng2ImgMaxService } from 'ng2-img-max';
import { CommonViewService } from '@services/common/common-view.service';


@Component({
	selector: 'app-item-update',
	templateUrl: './item-update.html'
})

export class ItemUpdateComponent implements OnChanges {

	userType : number;

	@Input() id : number;
	@Input() details : any;
	@Input() updateForm : FormGroup;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
	@ViewChild("fileInput") fileInput;
	@Input() itemStatus : any;

	isImageChanged : boolean = false;
  	imagePath : string = "assets/images/item-images/image/";
    isUploading : boolean = false;

    date : string = Date.now().toString();

  	imageErrorMessage : string;
	successMessage : string;
	errorMessage : any;


 	sizeList : any = [];
    uploadedImage: File;

	constructor(
		private fb : FormBuilder,
		private _requestService : RequestService,
		private _commonViewService : CommonViewService,
		private pageModuleService: PageModuleService,
      	private ng2ImgMax: Ng2ImgMaxService) {
		
		this.loadDropdown();
		this.userType = parseInt(this.pageModuleService.userType);
		this._requestService.action = "items";
		
	}


	private loadDropdown(): void {
    
    	this._commonViewService.getCommonList("sizes")
								 .subscribe(ddl => {this.sizeList = ddl;
								console.log(ddl); });  
								 console.log(this.sizeList);
 
 	} 

	onFileChange(event) {
		if(event.target.files.length > 0) {
		  let file = event.target.files[0];


		  if (file.type.includes("image")) {
		      if (file["size"] <= 7000000) {

		      	  this.isImageChanged = true;
		          this.isUploading = true;
		          this.imagePath = "assets/images/spinner.gif";

	              this.updateForm.get('ImageName').setValue(file.name); 
	              this.imageErrorMessage = null; 

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
		  this.updateForm.get('ImageName').setValue(''); 
		}
	}


	onSubmit() {
			let formData = this.updateForm.value;
			
			this._requestService.action = "items";

        this._requestService.updateRecord(this.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            this.successMessage = "Record Succesfully Updated";
			this.errorMessage = null;
			$("#saveModal").modal("hide");
        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.successMessage = null;
        });
	}

	onSelectStatus(value : boolean)
    {
		this.itemStatus = value;
		console.log(value);
	}

	ngOnChanges(changes : SimpleChanges)
	{
		this.isImageChanged = false;
		this.imagePath = "assets/images/item-images/image/";
		this.date = "?_=" + Date.now();

		this.successMessage = null;
		this.errorMessage = null;
	}

}

