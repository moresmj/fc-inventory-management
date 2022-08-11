import { Component, AfterViewInit, OnInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { InventoriesService } from '@services/inventories/inventories.service';
import { ApiBaseService } from '@services/api-base.service';
import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import { AppConstants } from '@common/app-constants/app-constants';

import { RequestService } from '@services/request.service';

declare var $:any;

@Component({
  selector:  'app-register-store-physical-count',
  templateUrl: './register-store-physical-count.html'
})


export class RegisterStorePhysicalCountComponent implements AfterViewInit, OnInit{

  template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

	serialNumber : string;
	code : string;

  itemDetail : any = [];
	newForm : FormGroup

  itemList : any;

	successMessage : string;
	errorMessage : string;

	serialErrorMessage : string;
	codeErrorMessage : string;

	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
      private _apiBaseService :ApiBaseService,
			private _inventoriesService : InventoriesService,
      private spinnerService: Ng4LoadingSpinnerService,
      private _requestService : RequestService
		) 
	{
      this._inventoriesService.action = "store/salesitem";
      this._apiBaseService.action = "imports/physicalcount/register/store";
      this._requestService.action = "imports/physicalcount/register/store";

		this.createForm();
		this.load();
	}


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

                let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId);
                let newQuantity = parseInt(itemDetails.controls.PhysicalCount.value) + parseInt(qty);
                itemDetails.controls.PhysicalCount.setValue(newQuantity);


                this.serialErrorMessage = null;
              }
              
            }
            else {
              this.serialErrorMessage = "Serial Number do not Exist.";
            }
          
            this.serialNumber = "";
            $("#itemCode").val("");
            $("#select2-itemCode-container").text("");
            this.successMessage = "";
            // this.clearData();
    
      }
    }

    getItemDetailsWithCode() {

      let id = $("#itemCode").val();

      if (id != '' && id.trim().length != 0) {

         let detail = this.itemList.find(x => x.itemId == id);
      
            if (detail != undefined) {
              let item = this.itemDetail.find(x => x.itemId == detail.itemId);
              if (item === undefined) {
                this.itemDetail.push(detail);
                this.addNewRow(detail,'');
                this.serialErrorMessage = null;
              }
              else {
                const control = this.newForm.controls['Details']['controls'];

                let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId);
                let newQuantity = parseInt(itemDetails.controls.PhysicalCount.value) + 1;
                itemDetails.controls.PhysicalCount.setValue(newQuantity);

                this.codeErrorMessage = null;
              }
            }
            else {
              this.codeErrorMessage = "Item Code do not Exist.";
            }
            this.clearData();
       
      }
      else{
        this.codeErrorMessage = "Please select Item";
      }

    }


    onSubmit() {
      let formData = this.newForm.value;
      console.log(formData);

      this._requestService.newRecord(formData)
        .subscribe(successCode => {
          this.newForm.reset();
          this.itemDetail = [];

          // Reset Array
          const arr = <FormArray>this.newForm.controls['Details'];
          arr.controls = [];

          this.successMessage = AppConstants.recordSaveSuccessMessage;;
          this.errorMessage = null;
          this.serialErrorMessage = null;
          this.codeErrorMessage = null;
        },
        error => {
          this.errorMessage = this._commonViewService.getErrors(error);
          this.successMessage = null;
          this.serialErrorMessage = null;
          this.codeErrorMessage = null;
        });

    }

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
        ItemId: new FormControl(data["itemId"], Validators.required),
        PhysicalCount: new FormControl(qty, [Validators.required, CustomValidator.quantity]),
        Remarks: ['']
      })
      // add new formgroup
      control.push(newItem);
    }


    deleteRow(index: number) {
      // control refers to your formarray
      const control = <FormArray>this.newForm.controls['Details'];
      // remove the chosen row
      this.itemDetail.splice(index, 1);
      control.removeAt(index);
    }

    get Details(): FormArray {
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
      this._commonViewService.getCommonList("inventories/store/salesitemdropdown").subscribe(dll => 
        { 
          this.itemList = dll;
          this.spinnerService.hide();

        },
        error =>{
                    this.spinnerService.hide();
        }
        );
    }

    createForm() {

      this.newForm = this.fb.group({
        Details: this.fb.array([])
      });
    }


    onSelectCode()
    {
        let id = $("#itemCode").val();
 

        if (id != '' && id.trim().length != 0){

            let item = this.itemList.find(x => x.itemId == id);
            console.log(item);
            $("#serial").val(item.serialNumber.toString());
            this.serialNumber = item.serialNumber.toString();
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null;
     


        }
    }


    removeMessages() {
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
    }


    clearData()
    {
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
