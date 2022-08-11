import { Component, AfterViewInit, OnInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { InventoriesService } from '@services/inventories/inventories.service';
import { OrderService } from '@services/order/order.service';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import { AppConstants } from '@common/app-constants/app-constants';

import { RequestService } from '@services/request.service';

import { CookieService } from 'ngx-cookie-service';

declare var $jquery: any;
declare var $:any;

@Component({
	selector : 'app-o-showroom-stock',
	templateUrl : './o-showroom-stock.html'
})


export class OrderShowroomStockComponent implements AfterViewInit, OnInit{

    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

	serialNumber : string;
	code : string;
    currentDate: any = new Date().toISOString().substring(0,10);

    itemDetail : any = [];
	newForm : FormGroup

    itemList : any;
    warehouseList : any = [];
    
    itemCodeList : any;
    tonalityList : any;
    deliveryModeList : any;

	successMessage : any;
	errorMessage : any;

	serialErrorMessage : string;
    codeErrorMessage : string;
    
    qtyErrorMessage : string;

    userType: any;

	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _orderService : OrderService,
			private _inventoriesService : InventoriesService,
            private spinnerService: Ng4LoadingSpinnerService,
            private _requestService : RequestService,
            private _cookieService : CookieService,
		) 
	{
        this._orderService.action = "orders/showroomstock";
        this._requestService.action = "transactions/orders/showroomstock"
        this.userType = this._cookieService.get('userType');

		this.createForm();
		this.load();
	}

    getWarehouseItems(id : any) {

        if (id != "") {
                this.spinnerService.show();
                const arr = <FormArray>this.newForm.controls['OrderedItems'];
                arr.controls = [];
                this.itemDetail= [];
                this._inventoriesService.action = "warehouse/items"

                // this._inventoriesService.action = "warehouse/items/code"

                this._inventoriesService.getListWithID(id)
                                            .subscribe( data =>
                                            {
                                                this.itemList = data;
                                                var uniqueCodes = [new Set(data.map(x => x.code))];

                                                this.itemCodeList = Array.from(uniqueCodes[0]);
                                                console.log(this.itemCodeList);
                                                this.spinnerService.hide();
                                            });

        }
        else {
            this.itemList = [];
            this.itemDetail= [];
            this.itemCodeList = [];

            const arr = <FormArray>this.newForm.controls['OrderedItems'];
                arr.controls = [];
        }
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


    onSelectCode2()
    {
        let code = $("#itemCode").val();
        

        if(code != '' && code.trim().length != 0)
        {
            var filteredTonality = this.itemList.filter(x => x.code == code && x.available > 0);
            this.tonalityList = filteredTonality;
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null;
        }
        else{
            this.tonalityList = null;
        }
    }

    getItemWithCode2() {
        let tonality = $("#tonality").val();
        let code = $("#itemCode").val();
        let qty = $("#qty").val();
        var item : any;

        if(code == '' || code === undefined)
        {
            return this.codeErrorMessage = "Please select item code";
        }
        
        if(tonality === 'ANY')
        {
            tonality = '';
        }
        if(tonality == '' && tonality.trim().length == 0)
        {
        
            if(qty == '')
            {
                qty = 1;
            }
            // Will not include old stock and mixtone tonality
            var filteredList = this.tonalityList.filter(p =>  p.tonality.toLowerCase() !="old stock");
            filteredList = filteredList.filter(p => p.tonality.toLowerCase() !="mixtone");
            var availableQuantities = filteredList.map(p => p.available);

            // Get the tonality with thes closest quantity that can accomodate the requested quantity
            var closestQty = this.closestQty(availableQuantities, qty);
            
            item = this.itemList.find(p => p.code == code && p.available == closestQty);
            item["isTonalityAny"] = true;
        }
        else{
            // If tonality and code has value will get item base on code and tonality
            item = this.itemList.find(p => p.code == code && p.tonality == tonality);
            item["isTonalityAny"] = false;
        }
        

        if (item != undefined && item.itemId != undefined) {

            if (item != undefined) {

                let itemonList = this.itemDetail.find(x => x.itemId == item.itemId);

                if(itemonList === undefined) {

                    if (this.itemDetail.length == 7) {
                        this.errorMessage = ["Only 7 items are allowed per purchase"];
                        $("#itemCode").val("");
                        $("#select2-itemCode-container").text("")
                        return;
                    }
                    else
                    {
                        this.itemDetail.push(item);
                        this.addNewRow(item,qty); 
                        this.serialErrorMessage = null;
                    }

                }
                else{
                    const control = this.newForm.controls['OrderedItems']['controls'];

                    let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );

                    let newQuantity = parseInt(itemDetails.controls.requestedQuantity.value) + parseInt(qty);

                    itemDetails.controls.requestedQuantity.setValue(newQuantity);

                    this.codeErrorMessage = null;
                }
            }
            else {
                this.codeErrorMessage = "The available quantity does not meet the requested quantity";
            }
            $("#itemCode").val("");
            $("#select2-itemCode-container").text("")
            this.errorMessage = null;
            this.successMessage = null;
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null; 
            $("#serial").val("");
            $("#qty").val("");
            $("#tonality").val("");
            this.serialNumber = null;
            this.tonalityList = [];


            }
            else
            {
                this.codeErrorMessage = "The available quantity does not meet the requested quantity";
            }
            

    }

    closestQty(arr, val)
    {
        // Will get the closest quantity available
       return arr.sort( (a, b) => Math.abs(val - a) - Math.abs(val - b)).find(x => x >= val);
    }

	getItemDetailsWithSerial() {
		if (this.serialNumber != null) {

            if(this.itemList === undefined)
            {

               return this.serialErrorMessage = AppConstants.selectWarehouse;
            }

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
                        if(item === undefined) {

                            if (this.itemDetail.length == 7) {
                                this.errorMessage = ["Only 7 items are allowed per purchase"];
                                this.serialNumber = null;
                                return;
                            }
                            else
                            {
                                this.itemDetail.push(detail);
                                this.addNewRow(detail,qty); 
                                this.serialErrorMessage = null;
                            }
                         
                        }
                        else{
                            const control = this.newForm.controls['OrderedItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.requestedQuantity.value) + parseInt(qty);
                            itemDetails.controls.requestedQuantity.setValue(newQuantity);

                            this.serialErrorMessage = null;
                        }
                        this.errorMessage = null;
                        this.successMessage = null;
                        this.serialErrorMessage = null; 
                        this.codeErrorMessage = null; 
					}
					else {
                        this.serialErrorMessage = "Serial Number do not Exist.";
                    }
                    this.serialNumber = "";
                    $("#itemCode").val("");
                    $("#select2-itemCode-container").text("");
	
            
		}
	}

	getItemDetailsWithCode() {

        let id = $("#itemCode").val();
           let detail = this.itemList.find(x => x.itemId == id);

		if (id != '' && id.trim().length != 0) {

					if (detail != undefined) {
 						let item = this.itemDetail.find(x => x.itemId == detail.itemId);
                        if(item === undefined) {

                            if (this.itemDetail.length == 7) {
                                this.errorMessage = ["Only 7 items are allowed per purchase"];
                                $("#itemCode").val("");
                                $("#select2-itemCode-container").text("")
                                return;
                            }
                            else
                            {
                                this.itemDetail.push(detail);
                                this.addNewRow(detail,''); 
                                this.serialErrorMessage = null;
                            }

                        }
                        else{
                            const control = this.newForm.controls['OrderedItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.requestedQuantity.value) + 1;
                            itemDetails.controls.requestedQuantity.setValue(newQuantity);
                            this.codeErrorMessage = null;
                        }
					}
					else {
                        this.codeErrorMessage = "Item Code do not Exist.";
                    }
                    $("#itemCode").val("");
                    $("#select2-itemCode-container").text("")
                    this.errorMessage = null;
                    this.successMessage = null;
                    this.serialErrorMessage = null; 
                    this.codeErrorMessage = null; 
                    $("#serial").val("");
                    this.serialNumber = null;


		}
      
	}

    onSubmit() {
        let formData = this.newForm.value;
        console.log(formData);

        this._requestService.newRecord(formData)
                .subscribe(successCode =>{
                    this.newForm.reset();
                    this.itemDetail = [];
                    this.itemList = [];
                    
                    // Reset Array
                    const arr = <FormArray>this.newForm.controls['OrderedItems'];
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

        // this._orderService.newRecord(formData)
        //     .subscribe(successCode =>{
        //         this.newForm.reset();
        //         this.itemDetail = [];
        //         this.itemList = [];
                
        //         // Reset Array
        //         const arr = <FormArray>this.newForm.controls['OrderedItems'];
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


    addNewRow(data: any,qty: any) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['OrderedItems'];

        qty = (qty != "") ? parseInt(qty) : 1;

        let newItem = this.fb.group({
                ItemId : new FormControl(data["itemId"],Validators.required),
                requestedQuantity : new FormControl(qty,[Validators.required,CustomValidator.requestedQuantity]),
                requestedRemarks : [''],
                availableQty: new FormControl(data.available),
                isTonalityAny : new FormControl(data.isTonalityAny),
        })
        // add new formgroup
        control.push(newItem);
    }

    

   


    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['OrderedItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get OrderedItems(): FormArray{
        return this.newForm.get('OrderedItems') as FormArray;
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
        this._commonViewService.getCommonList("warehouses/storewarehouse").subscribe(dll => {this.warehouseList = dll;});
        this._commonViewService.getCommonList("deliverytypes",true).subscribe(dll => { this.deliveryModeList = dll;});
	}

	createForm() {

		this.newForm = this.fb.group({
			warehouseId : new FormControl('',Validators.required),
			poDate : new FormControl(this.currentDate,Validators.required),
			remarks : new FormControl(''),
			OrderedItems : this.fb.array([])							
        });
        
        if(this.userType.toString() == "6")
        {
            this.newForm.addControl('poNumber',new FormControl('',Validators.required));
            this.newForm.addControl('isDealer',new FormControl(1));
        }
	}

    removeMessages() {

     
        
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
        this.qtyErrorMessage = null;
        if($("#qty").val() == "0")
        {
           this.qtyErrorMessage = "Quantity must be greater than 0"
        }
    }

	ngAfterViewInit() {
        var self = this;
        $(function () {
            //Initialize Select2 Elements
            $('#itemCode').select2();

            $("#itemCode").on("change", function () {
                self.onSelectCode2(); 


            });



        });
    }




}
