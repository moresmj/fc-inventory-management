import { Component, AfterViewInit, OnInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { RequestService } from '@services/request.service';
import { InventoriesService } from '@services/inventories/inventories.service';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { AppConstants } from '@common/app-constants/app-constants';
declare var $:any;

@Component({
	selector : 'app-rtv-purchase-returns',
	templateUrl : './rtv-purchase-returns.html'
})

export class RTVPurchaseReturnsComponent implements AfterViewInit, OnInit {

	serialNumber : string
	code : string;
    currentDate: any = new Date().toISOString().substring(0,10);

    itemDetail : any = [];
	newForm : FormGroup

    itemList : any = [];
    returnReasonList : any = [];
    warehouseName : string;

    isItemRemarkRequired : boolean = false;
	successMessage : string;
	errorMessage : string;

	serialErrorMessage : string;
	codeErrorMessage : string;


	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _apiBaseService : ApiBaseService,
            private _inventoriesService : InventoriesService,
            private _requestService : RequestService
		) 
	{
        this._apiBaseService.action = "returns/purchasereturn/warehousename";
        this._inventoriesService.action = "store/salesitem";

		this.createForm();
		this.load();
	}


	getItemDetailsWithSerial() {
		if (this.serialNumber != null) {

			if (this.serialNumber == "" || this.serialNumber.length  < AppConstants.skuLength) {

                this.serialNumber ="";

                this.serialErrorMessage = AppConstants.skuErrorMessage;
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
                        if(item === undefined) {
                            this.itemDetail.push(detail);
                            this.addNewRow(detail,qty); 
                            this.serialErrorMessage = null;
                        }
                        else{
                            const control = this.newForm.controls['PurchasedItems']['controls'];

                            let itemDetails = control.find(x => x.controls.itemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.goodQuantity.value) + parseInt(qty);
                            itemDetails.controls.goodQuantity.setValue(newQuantity);

                            this.serialErrorMessage = null;
                        }
                        this.errorMessage = null;
                        this.successMessage = null;
                        this.serialErrorMessage = null; 
                        this.codeErrorMessage = null; 
					}
					else {
                        this.serialErrorMessage = "Serial Number doesn't exist.";
                    }
                    // this.clearData();
                    $("#itemCode").val("");
                    $("#select2-itemCode-container").text("");
                    this.serialNumber = null;
				
		}
	}

	getItemDetailsWithCode() {

        let id = $("#itemCode").val();
        let detail = this.itemList.find(x => x.itemId == id);

		if (id != '' && id.trim().length != 0) {

					if (detail != undefined) {
 						let item = this.itemDetail.find(x => x.itemId == detail.itemId);
                        if(item === undefined) {
                            this.itemDetail.push(detail);
                            this.addNewRow(detail,''); 
                            this.serialErrorMessage = null;
                        }
                        else{
                            const control = this.newForm.controls['PurchasedItems']['controls'];

                            let itemDetails = control.find(x => x.controls.itemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.goodQuantity.value) + 1;
                            itemDetails.controls.goodQuantity.setValue(newQuantity);
                            this.codeErrorMessage = null;
                        }
					}
					else {
                        this.codeErrorMessage = "Item Code do not Exist.";
                    }
                    this.clearData();
 
		}
        
	}


    onSubmit() {
        let formData = this.newForm.value;
        console.log(formData);

        this._requestService.action = "returns/purchasereturn";
        this._requestService.newRecord(formData)
            .subscribe(successCode =>{
                this.newForm.reset();
                this.itemDetail = [];

                // Reset Array
                const arr = <FormArray>this.newForm.controls['PurchasedItems'];
                arr.controls = [];

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

    }

    addNewRow(data: any,qty : any) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['PurchasedItems'];

        if(qty != "")
        {
            qty =  parseInt(qty);
        }
        else{
            qty = 0;
        }


        let newItem = this.fb.group({
                poNumber : new FormControl(''),
                itemId : new FormControl(data["itemId"]),
                goodQuantity : new FormControl(qty,Validators.required),
                brokenQuantity : new FormControl(0),
                returnReason : new FormControl('',Validators.required),
                remarks : ['']
        },{

        validator : (control) => {
            var gQty = control.controls.goodQuantity;
            var bQty = control.controls.brokenQuantity;

            if(gQty != undefined && bQty != undefined){
                if(!(gQty.value > 0 || bQty.value > 0))
                    return {invalid: true};
                else if(gQty.value == null || bQty.value == null)
                    return {invalid: true};
            }
        }
        })
        // add new formgroup
        control.push(newItem);
    }


    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['PurchasedItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get PurchasedItems(): FormArray{
        return this.newForm.get('PurchasedItems') as FormArray;
    }

    ngOnInit() {
        this.newForm.valueChanges.subscribe(form => {
            this.removeMessages();
     	});
    }

	load() {
        this._commonViewService.getCommonList("inventories/store/salesitem").subscribe(dll => { this.itemList = dll; } );
        this._commonViewService.getCommonList("returnreason",true).subscribe(dll => { 
            this.returnReasonList = dll.filter(p => p['value'] != 1);
        });

        this._apiBaseService.action = "returns/purchasereturn/warehousename";
        this._apiBaseService.getList()
            .subscribe(name =>{ this.warehouseName = name; });


	}

	createForm() {

		this.newForm = this.fb.group({
			returnFormNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),
			remarks : new FormControl(''),
			PurchasedItems : this.fb.array([])							
		});
	}

    setReason(value : number, index :number) {

        let fields = ["remarks"];
        const control = <FormArray>this.newForm.controls['PurchasedItems'];
        
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() === "5") {
                    this.isItemRemarkRequired = true;
                    control["controls"][index]["controls"][fields[i]].setValidators([Validators.required]);
                    control["controls"][index]["controls"][fields[i]].updateValueAndValidity();  
            }
            else {
                    this.isItemRemarkRequired = false;
                    control["controls"][index]["controls"][fields[i]].setValidators(null);
                    control["controls"][index]["controls"][fields[i]].updateValueAndValidity();  
            }
        }
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

            let item = this.itemList.find(x => x.itemId == id);
            console.log(item);
            $("#serial").val(item.serialNumber.toString());
            this.serialNumber = item.serialNumber.toString();
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null;
     


        }
    }


    clearData(){
        $("#itemCode").val("");
                    $("#select2-itemCode-container").text("")
                    this.errorMessage = null;
                    this.successMessage = null;
                    this.serialErrorMessage = null; 
                    this.codeErrorMessage = null;
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