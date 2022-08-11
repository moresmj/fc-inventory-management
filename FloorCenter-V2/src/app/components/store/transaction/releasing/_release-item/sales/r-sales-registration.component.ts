import { Component, AfterViewInit, OnInit } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { InventoriesService } from '@services/inventories/inventories.service';

import { RequestService } from '@services/request.service';
import { Releasing } from '@models/releasing/releasing.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { AppConstants } from '@common/app-constants/app-constants';
declare var $:any;

@Component({
	selector : 'app-r-sales-registration',
	templateUrl: 'r-sales-registration.html'
})



export class ReleaseSalesRegistrationComponent  implements AfterViewInit{

    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

	serialNumber : string;
	code : string;

	newForm : FormGroup;

	itemList : any;
	itemDetail : any = [];


	serialErrorMessage : string;
	codeErrorMessage : string;

	successMessage : string;
	errorMessage : string;
	totalItems : any;
	totalQuantity : any;

    currentDate: any = new Date().toISOString().substring(0,10);


	constructor(private fb: FormBuilder,
				private _commonViewService : CommonViewService,
				private _itemService : ItemService,
				private _requestService : RequestService,
                private _inventoriesService : InventoriesService,
                private spinnerService: Ng4LoadingSpinnerService)
	{
          this._inventoriesService.action = "store/salesitem";
        //   this._requestService.action = "transactions/releasing/sales";


		this.createForm();
		this.load();
		this.totalItems = 0;
		this.totalQuantity = 0;


	}

	getItemDetailsWithSerial() {
		if (this.serialNumber != null) {

			if (this.serialNumber == ""|| this.serialNumber.length  < AppConstants.skuLength) {

                this.serialNumber = "";

                this.serialErrorMessage =AppConstants.skuErrorMessage;
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
                            const control = this.newForm.controls['SoldItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.itemId );
                            let newQuantity = parseInt(itemDetails.controls.Quantity.value) + parseInt(qty);
                            itemDetails.controls.Quantity.setValue(newQuantity);

                            this.serialErrorMessage = null;

                            /*for( let i = 0; i < control.length; i++)
							{
								this.totalQuantity = control[i].value.Quantity;
								console.log(control[i].value.Quantity);
							}*/
								this.totalQuantity = 0;

								control.map(x => {  
					this.totalQuantity = this.totalQuantity + x.value.Quantity; 

				});





                        }
                            this.clearData();
					}
					else {
                        this.serialErrorMessage = "Serial Number do not Exist.";
                    }
                    this.serialNumber = "";
	
		}
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
                            let newQuantity = parseInt(itemDetails.controls.Quantity.value) + 1;
                            itemDetails.controls.Quantity.setValue(newQuantity);
                            this.codeErrorMessage = null;
                            this.totalQuantity = 0;

					control.map(x => {  
					this.totalQuantity = this.totalQuantity + x.value.Quantity; 

					});
                        }
					}
					else {
                        this.codeErrorMessage = "Item Code do not Exist.";
                    }
                    this.clearData();


		}
 

        
	}



	onSubmit(){
        
		 let formData = this.newForm.value;
         formData.isSameDaySales = true;
         formData.dateUpdated = this.currentDate;
         formData.deliveryStatus= 1;
        console.log(formData)
        this._requestService.action = "transactions/releasing/sales";
         this._requestService.newRecord(formData)
         .subscribe(SuccessCode =>{
         	this.successMessage = "Sales successfully registered";
         	this.errorMessage = null;
            // this.createForm();
            console.log(SuccessCode["id"])

            this._requestService.action = "transactions/releasing/samedaysales";
            this._requestService.updateRecord(SuccessCode["id"], SuccessCode).subscribe(SuccessCode => console.log(SuccessCode))

            this.newForm.reset();
            this.itemDetail = [];
            const arr = <FormArray>this.newForm.controls['SoldItems'];
            arr.controls = [];
            this.totalItems = 0;
            this.totalQuantity = 0;

         },
         error =>{
         	this.errorMessage = this._commonViewService.getErrors(error);
         });

	}


	 addNewRow(data: any,qty : any) {
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
                Quantity : new FormControl(qty,[Validators.required]),
        })

        // add new formgroup
        control.push(newItem);

       this.totalItems = control.length;


		this.checkQuantity();

    }



    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['SoldItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
        this.totalItems = control.length;
        this.checkQuantity();
    }

    get SoldItems(): FormArray{
        return this.newForm.get('SoldItems') as FormArray;
    }



    checkQuantity(){
    	   const control = <FormArray>this.newForm.controls['SoldItems'];
    	   this.totalQuantity = 0;

	
							control.controls.map(x =>{  
					this.totalQuantity = this.totalQuantity + x.value.Quantity; 

				});
    }

	createForm() {

		this.newForm = this.fb.group({
			SINumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),
			releaseDate : new FormControl(this.currentDate,Validators.required),
            salesAgent : new FormControl('',Validators.required),
            isSameDaySales: new FormControl(true),
            SoldItems : this.fb.array([])	,	
            
            
           
            orNumber : new FormControl('',[ Validators.maxLength(50)]),
            dateUpdated : new FormControl(this.currentDate),
            drNumber : new FormControl('',[Validators.maxLength(50)]),
            remarks : new FormControl(''),
            ClientName : new FormControl('',Validators.required),
            address1: new FormControl(''),
            address2: new FormControl(''),
            address3: new FormControl(''),
            contactNumber : new FormControl(''),    
            deliveryType : new FormControl(''),
            transactionNo : new FormControl(),
            // releaseItems : this.fb.array([])
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

    clearData(){
        $("#itemCode").val("");
        $("#select2-itemCode-container").text("")
        this.errorMessage = null;
        this.successMessage = null;
        this.serialErrorMessage = null; 
        this.codeErrorMessage = null;
        this.serialNumber = null;
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
	}

	//clear error and success mesagge when input new data
	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
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