import { Component, AfterViewInit, OnInit } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { InventoriesService } from '@services/inventories/inventories.service';
import { RequestService } from '@services/request.service';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { AppConstants } from '@common/app-constants/app-constants';
declare var $:any;

@Component({
	selector : 'app-so-new-sales-order',
	templateUrl : './so-new-sales-order.html'
})

export class SalesOrderNewComponent implements AfterViewInit, OnInit {

    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

	serialNumber : string
	code : string;
    currentDate: any = new Date().toISOString().substring(0,10);

    itemDetail : any = [];
	newForm : FormGroup

    itemList : any;
    deliveryModeList : Dropdown[] = [];

	successMessage : string;
	errorMessage : string;

	serialErrorMessage : string;
	codeErrorMessage : string;


	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _requestService : RequestService,
			private _inventoriesService : InventoriesService,
            private spinnerService: Ng4LoadingSpinnerService
		) 
	{
        this._inventoriesService.action = "store/salesitem";
        this._requestService.action = "transactions/salesorder";

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

            let items = this.itemList;

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
                            const control = this.newForm.controls['SoldItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.quantity.value) + parseInt(qty);
                            itemDetails.controls.quantity.setValue(newQuantity);

                            this.serialErrorMessage = null;
                        }
                        this.clearData();
                    }
                    else {
                        this.serialErrorMessage = "Serial Number do not Exist.";
                    }
                    this.serialNumber = "";
		}
        else{
            this.serialErrorMessage = "Please enter Serial Number"
        }

        this.serialNumber = "";

	}

	getItemDetailsWithCode() {

        let id = $("#itemCode").val();

		if (id != '' && id.trim().length != 0) {

        let detail = this.itemList.find(x => x.itemId == id);

        if (detail != undefined) {
                        let item = this.itemDetail.find(x => x.itemId == detail.itemId);
                        if(item === undefined) {
                            this.itemDetail.push(detail);
                            this.addNewRow(detail,''); 
                            this.serialErrorMessage = null;
                        }
                        else{
                            const control = this.newForm.controls['SoldItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.quantity.value) + 1;
                            itemDetails.controls.quantity.setValue(newQuantity);
                            this.codeErrorMessage = null;
                        }
                    }
                    else {
                        this.codeErrorMessage = "Item Code do not Exist.";
                    }
                    this.clearData();

		}
        else{
            this.codeErrorMessage = "Please select an Item";
        }
        
	}


    onSubmit() {
        let formData = this.newForm.value;
        console.log(formData);

        this._requestService.newRecord(formData)
            .subscribe(successCode =>{
                this.newForm.reset();
                this.itemDetail = [];

                // Reset Array
                const arr = <FormArray>this.newForm.controls['SoldItems'];
                arr.controls = [];

                this.successMessage = "Records Successfully Saved";
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

    addNewRow(data: any, qty : any) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['SoldItems'];

        if(qty != "")
        {
            qty =  parseInt(qty);
        }
        else{
            qty = 1;
        }


        let newItem = this.fb.group({
                ItemId : new FormControl(data["itemId"],Validators.required),
                quantity : new FormControl(qty,[Validators.required,CustomValidator.quantity]),
                remarks : ['']
        })
        // add new formgroup
        control.push(newItem);
    }


    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['SoldItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get SoldItems(): FormArray{
        return this.newForm.get('SoldItems') as FormArray;
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
        this._commonViewService.getCommonList("inventories/store/salesitem").subscribe(dll => 
            { 
                this.itemList = dll;
                this.spinnerService.hide();
            },
            error =>{
                    this.spinnerService.hide();
            } 
            );
        this._commonViewService.getCommonList("deliverytypes",true).subscribe(dll => { 
            this.deliveryModeList = dll; 
            this.deliveryModeList = this.deliveryModeList.filter(x => x["value"] != 3);
        
        });
	}

	createForm() {

		this.newForm = this.fb.group({
			siNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),
			orNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),
			drNumber : new FormControl('',[Validators.maxLength(50)]),
			deliveryType : new FormControl('',Validators.required),
            salesDate : new FormControl(this.currentDate,Validators.required),   
			salesAgent : new FormControl('',Validators.required),	
			clientName : new FormControl('',Validators.required),	
            contactNumber : new FormControl(''),				
			address1 : new FormControl(''),
			address2 : new FormControl(''),
			address3 : new FormControl(''),
			SoldItems : this.fb.array([])							
		});
	}


    setDelivery(value : number) {

        let fields = ["contactNumber","address1","drNumber"];
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() === "2") {
                    this.newForm.controls[fields[i]].setValidators([Validators.required]);
                    this.newForm.controls[fields[i]].updateValueAndValidity();   
            }
            else {
                    this.newForm.controls[fields[i]].setValidators(null);
                    this.newForm.controls[fields[i]].updateValueAndValidity();  
            }
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

	removeMessages() {
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
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