import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { RequestService } from '@services/request.service';
import { PageModuleService } from '@services/common/pageModule.service';
import { ApiBaseService } from '@services/api-base.service';

import { CommonViewService } from '@services/common/common-view.service';


@Component({
	selector: 'app-company-update',
	templateUrl: './company-update.html'
})

export class CompanyUpdateComponent implements OnChanges {

	userType : number;
	module : string = "company";

	@Input() id : number;
	@Input() details : any;
	@Input() updateForm : FormGroup;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
  	@ViewChild("fileInput") fileInput;

	isImageChanged : boolean = false;
  	imagePath : string = "assets/images/company-images/image/";
    isUploading : boolean = false;

    date : string = Date.now().toString();

  	imageErrorMessage : string;
	successMessage : string;
	errorMessage : any;


 	sizeList : any = [];
    uploadedImage: File;

	constructor(
		private fb : FormBuilder,
		private _apiBaseService : ApiBaseService,
		private _commonViewService : CommonViewService,
		private pageModuleService: PageModuleService,
		private _requestService : RequestService) {
		
		this.loadDropdown();
		this.userType = parseInt(this.pageModuleService.userType);
		this._requestService.action = "companies";
		
	}


	private loadDropdown(): void {
    
    	this._commonViewService.getCommonList("sizes")
                     			.subscribe(ddl => {this.sizeList = ddl; });  
 
 	} 



	onSubmit() {
	    let formData = this.updateForm.value;

        this._requestService.updateRecord(this.id,formData)
        .subscribe(successCode =>{
            this.updatePage.emit("loadPageRecord");
            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;
        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.successMessage = null;
        });
	}

	ngOnChanges(changes : SimpleChanges)
	{

		this.successMessage = null;
		this.errorMessage = null;
	}

	onChange(ch : any){

		this.errorMessage = null;
		this.successMessage = null;
}

}

