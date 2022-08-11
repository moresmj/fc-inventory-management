import { Component, AfterViewInit, OnInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { ApiBaseService } from '@services/api-base.service';
import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';
import { AppConstants } from '@common/app-constants/app-constants';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import { RequestService } from '@services/request.service';

declare var $:any;

@Component({
  selector:  'app-register-physical-count',
  templateUrl: './register-physical-count.html'
})


export class RegisterPhysicalCountComponent implements AfterViewInit, OnInit{

    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

	serialNumber : string;
	code : string;

  itemDetail : any = [];
	newForm : FormGroup

  itemList : any = [];

	successMessage : string;
	errorMessage : any;

	serialErrorMessage : string;
	codeErrorMessage : string;

	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
      private _apiBaseService :ApiBaseService,
			private _itemService : ItemService,
      private spinnerService: Ng4LoadingSpinnerService,
      private _requestService : RequestService
		) 
	{
      this._apiBaseService.action = "imports/physicalcount/register/warehouse";
      this._requestService.action = "imports/physicalcount/register/warehouse";

		this.createForm();
		this.load();
	}

	/*getItemDetailsWithSerial() {
		if (this.serialNumber != null) {

			if (this.serialNumber == "" || this.serialNumber.length  < 9) {

                this.serialErrorMessage = "Serial number must be atleast 9 Digits";
                this.serialNumber = "";
                return;

            }

			this._itemService.getItemDetailsWithSerial(this.serialNumber)
				.subscribe(detail => {
					if (detail[0] != undefined) {
 						let item = this.itemDetail.find(x => x.serialNumber == detail[0]["serialNumber"]);
                        if(item === undefined) {
                            this.itemDetail.push(detail[0]);
                            this.addNewRow(detail[0]); 
                            this.serialErrorMessage = null;
                        }
                        else{
                            const control = this.newForm.controls['Details']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.id );
                            let newQuantity = parseInt(itemDetails.controls.PhysicalCount.value) + 1;
                            itemDetails.controls.PhysicalCount.setValue(newQuantity);
                            this.serialErrorMessage = null;


                        }
                            this.clearData();
					}
					else {
                        this.serialErrorMessage = "Serial Number do not Exist.";
                    }
                    
				},
                error =>{

                    this.serialNumber = "";

                    this.errorMessage = this._commonViewService.getErrors(error);
                    this.successMessage = null;
                    this.serialErrorMessage = null; 
                    this.codeErrorMessage = null;  
                });
		}
	}*/

    getItemDetailsWithSerial() {
      if (this.serialNumber != null) {

        if (this.serialNumber == "" || this.serialNumber.length  < AppConstants.skuLength) {

                this.serialErrorMessage = AppConstants.skuErrorMessage;
                this.serialNumber = "";
                return;
        }

            let serial  = this.serialNumber
            this.serialNumber = serial.substring(0,AppConstants.skuLength);
            let qty = serial.substring(AppConstants.skuLength);
            if(qty == "")
            {
               qty = "1";
            }

        let detail = this.itemList.find(x => x.serialNumber == this.serialNumber);

        
            if (detail != undefined) {
              let item = this.itemDetail.find(x => x.serialNumber == detail.serialNumber);
              if (item === undefined) {
                this.itemDetail.push(detail);
                this.addNewRow(detail,qty);
                this.serialErrorMessage = null;
              }
              else {
                const control = this.newForm.controls['Details']['controls'];

                let itemDetails = control.find(x => x.controls.ItemId.value == item.id);
                let newQuantity = parseInt(itemDetails.controls.PhysicalCount.value) + parseInt(qty);
                itemDetails.controls.PhysicalCount.setValue(newQuantity);


                this.serialErrorMessage = null;
              }
              
            }
            else {
                this.clearData();
             return this.serialErrorMessage = "Serial Number do not Exist.";
            }
            this.serialNumber = "";
            this.clearData();
    
      }
    }

    getItemDetailsWithCode() {

      let id = $("#itemCode").val();

      if (id != '' && id.trim().length != 0) {

         let detail = this.itemList.find(x => x.id == id);
      
            if (detail != undefined) {
              let item = this.itemDetail.find(x => x.id == detail.id);
              if (item === undefined) {
                this.itemDetail.push(detail);
                this.addNewRow(detail,'');
                this.serialErrorMessage = null;
              }
              else {
                const control = this.newForm.controls['Details']['controls'];

                let itemDetails = control.find(x => x.controls.ItemId.value == item.id);
                let newQuantity = parseInt(itemDetails.controls.PhysicalCount.value) + 1;
                itemDetails.controls.PhysicalCount.setValue(newQuantity);

                this.codeErrorMessage = null;
              }
            }
            else {
              return this.codeErrorMessage = "Item Code do not Exist.";
            }
            this.clearData();
       
      }
      else{
        this.codeErrorMessage = "Please select Item";
      }

    }

	/*getItemDetailsWithCode() {

        let id = $("#itemCode").val();

		if (id != '' && id.trim().length != 0) {
			this._itemService.getItemDetailsWithId(id)
				.subscribe(detail => {
					if (detail[0] != undefined) {
 						let item = this.itemDetail.find(x => x.code == detail[0]["code"]);
                        if(item === undefined) {
                            this.itemDetail.push(detail[0]);
                            this.addNewRow(detail[0]); 
                            this.serialErrorMessage = null;
                        }
                        else{
                            const control = this.newForm.controls['Details']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.id );
                            let newQuantity = parseInt(itemDetails.controls.PhysicalCount.value) + 1;
                            itemDetails.controls.PhysicalCount.setValue(newQuantity);
                            this.codeErrorMessage = null;
                        }
					}
					else {
                        this.codeErrorMessage = "Item Code do not Exist.";
                    }
                    this.clearData();
				}); 
		}
 

        
	}*/

    onSubmit() {
        let formData = this.newForm.value;
        console.log(formData);
    
        this._requestService.newRecord(formData)
            .subscribe(successCode =>{
                this.newForm.reset();
                this.itemDetail = [];

                // Reset Array
                const arr = <FormArray>this.newForm.controls['Details'];
                arr.controls = [];

                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.errorMessage = null;      
                this.serialErrorMessage = null;  
                this.codeErrorMessage = null;  
            },
            error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
                this.errorMessage[0] = AppConstants.itemListError;
                this.successMessage = null;
                this.serialErrorMessage = null; 
                this.codeErrorMessage = null;  
            });

    }


   /* addNewRow(data: any) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['Details'];

        let newItem = this.fb.group({
                ItemId : new FormControl(data["id"],Validators.required),
                PhysicalCount : new FormControl(1,[Validators.required,CustomValidator.requestedQuantity]),
                Broken : ['0']
        })
        // add new formgroup
        control.push(newItem);
    }*/


    addNewRow(data: any,qty : any) {
      // control refers to your formarray
      const control = <FormArray>this.newForm.controls['Details'];


      if(qty != "")
        {
            qty =  parseInt(qty);
        }
        else{
            qty = 1;
        }

      let newItem = this.fb.group({
        ItemId: new FormControl(data["id"], Validators.required),
        PhysicalCount: new FormControl(qty, [Validators.required, CustomValidator.quantity]),
        Remarks: ['',Validators.required]
      })
      // add new formgroup
      control.push(newItem);
    }


    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['Details'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get Details(): FormArray{
        return this.newForm.get('Details') as FormArray;
    }


    ngOnInit() {
        this.newForm.valueChanges.subscribe(form => {
            this.errorMessage = null;
            this.successMessage = null;
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null;
      });
    }

	load() {
        this.spinnerService.show();
		this._commonViewService.getCommonList("items/itemfordropdown").subscribe(dll => {
            this.itemList = dll;
            this.spinnerService.hide();
        });
	}

	createForm() {

		this.newForm = this.fb.group({
			Details : this.fb.array([])							
		});
	}

    removeMessages() {
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
    }


    onSelectCode()
    {
        let id = $("#itemCode").val();
 

        if (id != '' && id.trim().length != 0){

            let item = this.itemList.find(x => x.id == id);
            console.log(item);
            $("#serial").val(item.serialNumber.toString());
            this.serialNumber = item.serialNumber.toString();
            this.codeErrorMessage = null;
            this.serialErrorMessage = null; 
     


        }
    }

    clearData(){
        $("#itemCode").val("");
                    $("#select2-itemCode-container").text("")
                    this.errorMessage = null;
                    this.successMessage = null;
                    this.serialErrorMessage = null; 
                    this.codeErrorMessage = null; 
                    $("#serial").val("");
                    this.serialNumber = null;
    }

	ngAfterViewInit() {

        var self = this;
        $(function () {
            //Initialize Select2 Elements
            $('#itemCode').select2();

               $("#itemCode").on("change", function () {
                self.onSelectCode(); 


            });
        });
    }




}
