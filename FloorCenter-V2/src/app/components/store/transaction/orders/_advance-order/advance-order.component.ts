import { Component, AfterViewInit, OnInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { InventoriesService } from '@services/inventories/inventories.service';
import { OrderService } from '@services/order/order.service';
import { RequestService } from '@services/request.service';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import { AppConstants } from '@common/app-constants/app-constants';

import { BaseComponent } from '@common/base.component';

declare var $:any;
declare var $jquery: any;

@Component({
	selector : 'app-advance-order',
	templateUrl : './advance-order.html'
})

export class AdvanceOrderComponent extends BaseComponent implements AfterViewInit, OnInit {

 

	serialNumber : string
	code : string;
    currentDate: any = new Date().toISOString().substring(0,10);

    itemDetail : any = [];
	newForm : FormGroup

    itemList : any;

    warehouseList : any = [];
    allDeliveryModeList : any = [];
    deliveryModeList : any = [];
    allPaymentModelList : any = [];
    paymentModeList: any = [];
	successMessage : any;
	errorMessage : any;

	serialErrorMessage : string;
    codeErrorMessage : string;


    qtyErrorMessage : string;
    
    itemCodeList : any;
    tonalityList : any;

    whInvetoryList: any;
    mainItemList : any;

    selectedType: any;


	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _orderService : OrderService,
            private _inventoriesService : InventoriesService,
            private spinnerService: Ng4LoadingSpinnerService,
            private _requestService : RequestService
		) 
	{
        super();
        this._orderService.action = "orders/forclient";
        this._requestService.action = "transactions/orders/advanceorder";

		this.createForm();
		this.load();
	}

    getWarehouseItems(id : any) {
      

        if (id != "") {
                this.spinnerService.show();

                let warehouseDetails = this.warehouseList.filter(x => x["id"] == id);
                // Warehouse.vendor = true [Can only select showroom pickup]
                this.deliveryModeList = (warehouseDetails[0]["vendor"]) ? this.allDeliveryModeList.filter(x => x["value"] == 3) : this.allDeliveryModeList;

                const arr = <FormArray>this.newForm.controls['AdvanceOrderDetails'];
                arr.controls = [];
                this.itemDetail= [];
                this._inventoriesService.action = "warehouse/items"
                this._inventoriesService.getListWithID(id).subscribe( data => {

                                                                        this.itemList = data;
                                                                        this.whInvetoryList = data;

                                                                        this.spinnerService.hide();

                                                                        var uniqueCodes = [new Set(data.map(x => x.code))];

                                                                        this.itemCodeList = Array.from(uniqueCodes[0]);
                                                                        // console.log("item: ", this.itemList)
                                                                        this.onSelectItemType(1)
                                                                    });
        }
        else {
            this.itemList = [];
            this.itemDetail= [];
            this.whInvetoryList = [];
            $("#itemType").val("");
            this.selectedType = null;

            const arr = <FormArray>this.newForm.controls['AdvanceOrderDetails'];
                arr.controls = [];
                this.onSelectItemType(0)
        }
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
            if(qty == "" || qty == undefined)
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
                            const control = this.newForm.controls['AdvanceOrderDetails']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.quantity.value) + parseInt(qty);
                            itemDetails.controls.quantity.setValue(newQuantity);

                            this.serialErrorMessage = null;
                        }
                        this.clearData();
                    }
                    else {
                        this.serialErrorMessage = AppConstants.serialDoNotExist;
                    }
                    this.serialNumber = "";
    
            
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
            //remove filtering for available items
            var filteredTonality = this.itemList.filter(x => x.code == code);
            this.tonalityList = filteredTonality;
            this.serialErrorMessage = null; 
            this.codeErrorMessage = null;
        }
        else{
            this.tonalityList = null;
        }
    }


    getItemWithCode2() {
        this.errorMessage = [];
        let tonality = $("#tonality").val();
        let code = $("#itemCode1").val();
        let qty = $("#qty").val();
        console.log("item code val: ", code)
        var item : any;

        //if item is custom item
        if(this.selectedType == 1)
        {
            let size = $("#size").val();

            if(code == '' || code === undefined)
            {
                 this.errorMessage.push(AppConstants.selectItemCodeErr);
            }
            if(size == '' || size === undefined)
            {
                this.errorMessage.push(AppConstants.selectItemsizeErr)
            }
            if((this.errorMessage != '' || this.errorMessage.length > 0))
            {
                return;
            }

            

            if(tonality === 'ANY')
            {
                tonality = '';
            }
            if(qty == "" || qty == undefined)
            {
                qty = 1;
            }
            

            
            let sizeName = this.sizeList.find(p => p.id == size);
            const customItem = { 
                                    code: code,
                                    tonality : tonality,
                                    quantity : qty,
                                    sizeId : size,
                                    sizeName : sizeName.name,
                                    isCustom : true,
                                    itemId : '',  
                                };

            let itemonList = this.itemDetail.find(x => x.code == customItem.code && x.sizeId == customItem.sizeId);

            if(itemonList === undefined) {

                if (this.itemDetail.length == 7) {
                    this.errorMessage = [AppConstants.maxItemOnListErr];
                    $("#itemCode").val("");
                    $("#select2-itemCode-container").text("")
                    return;
                }
                else
                {
                    this.itemDetail.push(customItem);
                    this.addNewRow(customItem,qty); 
                    this.serialErrorMessage = null;
                }

            }
            else{
                this.errorMessage = [AppConstants.itemAlreadyOnListErr];
            }
         
        

        }
        //for existing item
        else
        {

        
   

            if(code == '' || code === undefined)
            {
                return this.codeErrorMessage = AppConstants.selectItemCodeErr;
            }
            if(tonality === 'ANY')
            {
                tonality = '';
            }

            if(tonality == '' && tonality.trim().length == 0)
            {
            
                if(qty == "" || qty == undefined)
                {
                    qty = 1;
                }
                // Will not include old stock and mixtone tonality
                var filteredList = this.tonalityList.filter(p =>  p.tonality.toLowerCase() !="old stock");
                filteredList = filteredList.filter(p => p.tonality.toLowerCase() !="mixtone");
                var availableQuantities = filteredList.map(p => p.available);

                // Get the tonality with thes closest quantity that can accomodate the requested quantity
                var closestQty = this.closestQty(availableQuantities, qty);
                
                // item = this.itemList.find(p => p.code == code && p.available == closestQty);
                 item = this.itemList.find(p => p.code == code);
            }
            else{
                // If tonality and code has value will get item base on code and tonality
                item = this.itemList.find(p => p.code == code && p.tonality == tonality);
            }
            

            if (item != undefined && item.itemId != undefined) {

                if (item != undefined) {

                    let itemonList = this.itemDetail.find(x => x.itemId == item.itemId);

                    if(itemonList === undefined) {

                        if (this.itemDetail.length == 7) {
                            this.errorMessage = [AppConstants.maxItemOnListErr];
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
                        const control = this.newForm.controls['AdvanceOrderDetails']['controls'];

                        let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );

                        let newQuantity = parseInt(itemDetails.controls.quantity.value) + 1;

                        itemDetails.controls.quantity.setValue(newQuantity);

                        this.codeErrorMessage = null;
                    }
                }
                else {
                    this.codeErrorMessage = AppConstants.availQtyNotSufficientErr;
                }

                this.clearData();

                }
                else
                {
                    this.codeErrorMessage = AppConstants.availQtyNotSufficientErr;
                }
        }
            

    }

    closestQty(arr, val)
    {
        
       return arr.sort((a,b) => a > b).find(x => x > val);
    }



    onSubmit() {
        this._requestService.action = "transactions/orders/advanceorder";
        let formData = this.newForm.value;
        console.log(formData);

        this._requestService.newRecord(formData)
        .subscribe(successCode => {
                this.newForm.reset();
                this.itemDetail = [];
                this.itemList = [];
                this.itemCodeList = [];
                this.tonalityList = [];
                $("#tonality").val("");
                $("#size").val("");
                $("#itemType").val("");

                // Reset Array
                const arr = <FormArray>this.newForm.controls['AdvanceOrderDetails'];
                arr.controls = [];

                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.errorMessage = null;      
                this.serialErrorMessage = null;  
                this.codeErrorMessage = null;
                this.selectedType = null;

        },
        error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
                this.successMessage = null;
                this.serialErrorMessage = null; 
                this.codeErrorMessage = null;  
        })

       
    }

    addNewRow(data: any,qty : any) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['AdvanceOrderDetails'];

        if(qty != "")
        {
            qty =  parseInt(qty);
        }
        else{
            qty = 1;
        }

        if(data["isCustom"] === true)
        {
            let newItem = this.fb.group({
                            itemId: new FormControl(''),
                            quantity : new FormControl(qty,[Validators.required,CustomValidator.quantity]),
                            code: new FormControl(data["code"]),
                            sizeId : new FormControl(data["sizeId"]),
                            tonality : new FormControl(data["tonality"]),
                            requestedRemarks : [''],
                            availableQty: new FormControl(''),
                            isCustom : new FormControl(data["isCustom"]),
                    })
            control.push(newItem);
        }
        else{
            let newItem = this.fb.group({
                ItemId : new FormControl(data["itemId"],Validators.required),
                quantity : new FormControl(qty,[Validators.required,CustomValidator.quantity]),
                code: new FormControl(""),
                sizeId : new FormControl(""),
                tonality : new FormControl(""),
                requestedRemarks : [''],
                availableQty: new FormControl(data.available),
                isCustom : new FormControl(false),
                })
                // add new formgroup
                control.push(newItem);
        }
        
    }


    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['AdvanceOrderDetails'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get AdvanceOrderDetails(): FormArray{
        return this.newForm.get('AdvanceOrderDetails') as FormArray;
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
        this._commonViewService.getCommonList("warehouses/storewarehouse").subscribe(dll => { this.warehouseList = dll; } );
        
        this._commonViewService.getCommonList("sizes").subscribe(ddl => { this.sizeList = ddl; console.log(ddl); });
        
        this._commonViewService.getCommonList("itemtypes")
                                .subscribe(ddl => {
                                     this.itemTypeList = ddl; 
                                    });

        this._commonViewService.getCommonList("paymentModes",true)
                     			.subscribe(ddl => { 
                     				this.paymentModeList = ddl; 
                                 });
                                 
                                 
            this._requestService.action = "items"
            this._requestService.getList().subscribe( data => {

                                                                    this.mainItemList = data;
                                                                });
		}

	createForm() {

		this.newForm = this.fb.group({
			warehouseId : new FormControl('',Validators.required),
            clientName : new FormControl('',Validators.required),
            siNumber : new FormControl(''),
            orNumber: new FormControl(''),
            salesAgent: new FormControl('',Validators.required),
            paymentMode : new FormControl('', Validators.required),
            address1 : new FormControl('',Validators.required),
            address2 : new FormControl(''),
            address3 : new FormControl(''),
            remarks : new FormControl(''),
            contactNumber : new FormControl('',Validators.compose([Validators.required,Validators.minLength(6)])),
            itemCodefcn : new FormControl(''),
			AdvanceOrderDetails : this.fb.array([]),							
		});
	}

    setDelivery(value : number) {

        let fields = ["contactNumber","address1"];
        this.paymentModeList = (value.toString() == "3") ? this.allPaymentModelList : this.allPaymentModelList.filter(x => x["value"] != 11);
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() === "2") {
                if(fields[i] == "contactNumber"){
                    this.newForm.controls[fields[i]].setValidators(Validators.compose([Validators.required,Validators.minLength(6)]));
                    this.newForm.controls[fields[i]].updateValueAndValidity();    
                }
                else{
                    this.newForm.controls[fields[i]].setValidators([Validators.required]);
                    this.newForm.controls[fields[i]].updateValueAndValidity(); 
                }
            }  
            else {
                    this.newForm.controls[fields[i]].setValidators(null);
                    this.newForm.controls[fields[i]].updateValueAndValidity();
             
            }
        }
    }


    onSelectItemType(value : any)
    {

        this.selectedType = value === "" ? null : value;
        this.clearData();

        if(value == 1)
        {
            var uniqueCodes = [new Set(this.mainItemList.map(x => x.code))];

            this.itemCodeList = Array.from(uniqueCodes[0]);
        }
        else{
            var uniqueCodes = [new Set(this.whInvetoryList.map(x => x.code))];

            this.itemCodeList = Array.from(uniqueCodes[0]);
        }
       
        var self = this;
        $(function () 
        {
            //Initialize Select2 Elements
            $('#itemCode').select2();
            $("#itemCode").on("change", function () 
            { 
                self.onSelectCode2();
             
            });
        });
        

    }





	removeMessages() {
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
        this.qtyErrorMessage = null;

        if($("#qty").val() == "0")
        {
           this.qtyErrorMessage = AppConstants.qtyMustGreaterThanZeroErr;
        }
	}

    clearData(){
        $("#itemCode").val("");
        $("#select2-itemCode-container").text("")
        $("#serial").val("");
        $("#qty").val("");
        $("#tonality").val("");
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null; 
        this.serialNumber = null;
        this.tonalityList = [];
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
