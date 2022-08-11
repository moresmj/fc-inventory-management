import { Component, AfterViewInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { RequestService } from '@services/request.service';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { AppConstants } from '@common/app-constants/app-constants';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import { CookieService } from 'ngx-cookie-service';

declare var $:any;

@Component({
	selector : 'app-o-interbranch',
	templateUrl : './o-interbranch.html'
})

export class OrderInterbranchComponent implements AfterViewInit {
    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'
    serialNumber : string = "";
    code : string;
    currentDate: any = new Date().toISOString().substring(0,10);

    itemDetail : any = [];
    newForm : FormGroup

    hideInterBranch :any = [23, 24, 25, 27, 28, 29];

    storeTransferOrders : any = [];
    itemList : any = [];
    storeList : any = [];
    deliveryModeList : any = [];
    paymentModeList: any = [];
    allPaymentModelList : any = [];

    successMessage : any;
    errorMessage : any;

    serialErrorMessage : string;
    codeErrorMessage : string;

    userType : any;


    constructor(
        private fb : FormBuilder,
        private _commonViewService : CommonViewService,
        private _apiBaseService : ApiBaseService,
        private _requestService : RequestService,
        private spinnerService: Ng4LoadingSpinnerService,
        private _cookieService : CookieService,
    ) 
    {
        this.load();
        this.userType = this._cookieService.get('userType');
        this.createForm();
        
    }


    getItemDetailsWithSerial() {


        if (this.serialNumber == "" || this.serialNumber.length  < AppConstants.skuLength) {

            this.serialErrorMessage = AppConstants.skuErrorMessage;
            this.serialNumber = "";
            return;

        }
        this._apiBaseService.action = 'inventories/store/interbranch';
        let param = { "serialNumber" : this.serialNumber };

        this.getItemDetails(param);
        this.clearData();


    }

    getItemDetailsWithCode() {

        let itemId = $("#itemCode").val();

        if (itemId != '' && itemId.trim().length != 0) {

          this._apiBaseService.action = 'inventories/store/interbranch';
          let param = { "id" : itemId };

          this.getItemDetails(param);
          
          this.clearData();
        }

    } 

    getItemDetails(param : any) {

          this._apiBaseService.action = 'inventories/store/interbranch';

          this._apiBaseService.getListWithParam(param)
            .subscribe(detail => 
              { 
                if (detail != null) {
                      let item = this.itemDetail.find(x => x["id"] == detail["id"]);
                      if(item === undefined) {

                          if (this.itemDetail.length == 7) {
                              this.errorMessage = ["Only 7 items are allowed per purchase"];
                              this.serialNumber = null;
                              return;
                          }
                          else
                          {

                            // HIDE AVALABLES OF INTERBRANCH NOT OWENED BY COMPANY                      
                            for( let i =0; i < detail.stores.length; i++){
                                for( let ii =0; ii< this.hideInterBranch.length; ii++){
                                    if(detail.stores[i].companyId == this.hideInterBranch[ii]){
                                        detail.stores[i].available = null
                                    }
                                }
                            }

                            this.itemDetail.push(detail);
                            this.addNewRow(detail); 
                            this.serialErrorMessage = null;
                          }
                          
                      }
                      // else
                      //{
                      //    const control = this.newForm.controls['TransferOrders']['controls'];

                      //    let itemDetails = control.find(x => x.controls.ItemId.value == item["id"]);
                      //    let newQuantity = parseInt(itemDetails.controls.requestedQuantity.value) + 1;
                      //    itemDetails.controls.requestedQuantity.setValue(newQuantity);

                      //    this.serialErrorMessage = null;
                      //}
                }
                else {
                      this.errorMessage = ["No Store have this item."];
                }
                this.serialNumber = "";
              },
              error =>{
                  this.serialNumber = "";

                  this.errorMessage = this._commonViewService.getErrors(error);
                  this.successMessage = null;
                  this.serialErrorMessage = null; 
                  this.codeErrorMessage = null;  
              });
    }



    onCheck(event,fGroup : any, x : number) {
        let value = parseInt(event.currentTarget.value);

        if (event.target.checked) {
            if (this.storeTransferOrders.length == 0) {
                this.storeTransferOrders.push(value);
            }
            else
            {
                if (this.storeTransferOrders.indexOf(value) == -1) {
                    this.storeTransferOrders.push(value);
                }
            }
        }
        else
        {
            let index = this.storeTransferOrders.indexOf(value)

            if (index != -1) {
              this.storeTransferOrders.splice(index, 1);
            }
        }


        let fields = ["quantity"];
        for(let i = 0; i < fields.length; i++)
        {
            if (event.target.checked) {
                fGroup.controls[fields[i]].setValidators([Validators.required, CustomValidator.requestedQuantity]);
                fGroup.controls[fields[i]].updateValueAndValidity();   
            }
            else {
                fGroup.controls[fields[i]].setValidators(null);
                fGroup.controls[fields[i]].updateValueAndValidity();  
            } 
        } 
    }


    onSubmit() {

      let formData = this.newForm.value;
      console.log(formData);
      let storeOrder = [];
      let itemOrder = [];

      // Compile All Item Order
      for (let i = 0; i < formData["TransferOrders"].length; i++) {
        
          let order = formData["TransferOrders"][i]["TransferredItems"];

          // Get Ordering of item by checking
          for (let x = 0; x < order.length; x++) {
              if (order[x]["isSelected"]) {
                  itemOrder.push(order[x]);
              }  
          }
      }

      // 
      for(let i = 0; i < this.storeTransferOrders.length; i++)
      {

         let StoreId = this.storeTransferOrders[i];
         storeOrder.push(
            {
              StoreId : StoreId,
              TransferredItems : itemOrder.filter(p => p["StoreId"] == StoreId)
            }
          );

        }

        formData["TransferOrders"] = storeOrder;
        this._apiBaseService.action = 'transfer';
        this._requestService.action ="transfer";

        this._requestService.newRecord(formData)
            .subscribe(successCode =>{
                this.newForm.reset();
                this.itemDetail = [];

                if(this.userType.toString() == "6")
                {    
                    this.newForm.controls.deliveryType.setValue(1);
                }

                // Reset Array
                const arr = <FormArray>this.newForm.controls['TransferOrders'];
                arr.controls = [];
                this.storeTransferOrders  = [];

                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.errorMessage = null;      
                this.serialErrorMessage = null;  
                this.codeErrorMessage = null;  
            },
            error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
                this.successMessage = null;
                this.serialErrorMessage = null; 
                this.codeErrorMessage = null;
            });




        // this._apiBaseService.newRecord(formData)
        //     .subscribe(successCode =>{
        //         this.newForm.reset();
        //         this.itemDetail = [];

        //         // Reset Array
        //         const arr = <FormArray>this.newForm.controls['TransferOrders'];
        //         arr.controls = [];

        //         this.successMessage = "Records Succesfully Saved";
        //         this.errorMessage = null;      
        //         this.serialErrorMessage = null;  
        //         this.codeErrorMessage = null;  
        //     },
        //     error =>{
        //         this.errorMessage = this._commonViewService.getErrors(error);
        //         this.successMessage = null;
        //         this.serialErrorMessage = null; 
        //         this.codeErrorMessage = null;  
        //     });


    }

    addNewRow(data: any) {

        const control = <FormArray>this.newForm.controls['TransferOrders'];

        let newItem = this.fb.group({
            ItemId: new FormControl(data["id"], Validators.required),
            TransferredItems: this.fb.array([])
        })
        control.push(newItem);

        for (let i = 0; i < data["stores"].length; i++) {
            this.addNewRowItems(this.getTransferredItems(control.controls[this.itemDetail.length - 1]), data["stores"][i], data["id"]);
        }


    }

    addNewRowItems(parentControl : any, data : any, itemId : any)  {

        let newItem = this.fb.group({
            ItemId: new FormControl(itemId),
            StoreId: new FormControl(data["storeId"]),
            quantity: new FormControl(''),
            isSelected: [false]
        })
        parentControl.push(newItem);
    }

    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['TransferOrders'];
        // remove the chosen row
        this.itemDetail.splice(index, 1);
        control.removeAt(index);
    }

    get TransferOrders(): FormArray {
        return this.newForm.get('TransferOrders') as FormArray;
    }

    getTransferredItems(formGroup : any) : FormArray  {
        return formGroup.get('TransferredItems') as FormArray;
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
        this._commonViewService.getCommonList("deliverytypes", true).subscribe(dll => { this.deliveryModeList = dll; });
        this._commonViewService.getCommonList("paymentmodes", true).subscribe(dll => {
             this.allPaymentModelList = dll; 
             if(this.userType.toString() == "6")
             {
                 this.setDelivery(1);
             }
            });
        this._commonViewService.getCommonList("items").subscribe(dll => { 
            this.itemList = dll; 
            this.spinnerService.hide();
        });
    }

    createForm() {

        this.newForm = this.fb.group({
            poDate: new FormControl(this.currentDate, Validators.required),
            deliveryType: new FormControl('', Validators.required),
            clientName: new FormControl('', Validators.required),
            paymentMode: new FormControl('', Validators.required),
            contactNumber: new FormControl('',Validators.minLength(6)), 
            address1: new FormControl(''),
            address2: new FormControl(''),
            address3: new FormControl(''),
            remarks: new FormControl(''),
            salesAgent: new FormControl('', Validators.required),
            TransferOrders: this.fb.array([])
        });

        if(this.userType.toString() == "6")
        {
            this.newForm.addControl('poNumber',new FormControl('',Validators.required));
            this.newForm.controls.deliveryType.setValue(1);
            this.newForm.addControl('isDealer',new FormControl(1));

        }
    }


    setDelivery(value : number) {
        console.log(value);

        let fields = ["contactNumber","address1"];
        this.paymentModeList = (value.toString() == "3") ? this.allPaymentModelList : this.allPaymentModelList.filter(x => x["value"] != 11);
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() === "2") {
                if(fields[i] == "contactNumber"){
                    this.newForm.controls[fields[i]].setValidators(Validators.compose([Validators.required, Validators.minLength(6)]));
                    this.newForm.controls[fields[i]].updateValueAndValidity();    
                }
                else{
                    this.newForm.controls[fields[i]].setValidators([Validators.required]);
                    this.newForm.controls[fields[i]].updateValueAndValidity(); 
                }
        }   
        else {
            if(fields[i] == "contactNumber"){
                this.newForm.controls[fields[i]].setValidators(Validators.compose([Validators.minLength(6)]));
            }
            else{
                this.newForm.controls[fields[i]].setValidators(null);
                this.newForm.controls[fields[i]].updateValueAndValidity();
            }  

        }
        }
    }



    onSelectCode()
    {
        let id = $("#itemCode").val();
 

        if (id != '' && id.trim().length != 0){

            let item = this.itemList.find(x => x.id == id);
            console.log(item);
            $("#serial").val(item.serialNumber.toString());
            this.serialNumber = item.serialNumber.toString();
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null;
     


        }
    }

    clearData(){
          $("#itemCode").val("");
          $("#select2-itemCode-container").text("");
          $("#serial").val("");
          this.serialNumber = null;
    }

    removeMessages() {
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
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
