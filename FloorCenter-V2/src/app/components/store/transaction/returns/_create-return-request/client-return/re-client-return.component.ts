import { Component, AfterViewInit, OnInit } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

//import { ReleasingService } from '@services/releasing/releasing.service';
//import { Releasing } from '@models/releasing/releasing.model';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';



declare var $:any;

@Component({
	selector : 'app-re-client-return',
	templateUrl: 're-client-return.html'
})



export class ClientReturnRegistration{

	serialNumber : string;
	code : string;

	newForm : FormGroup;

	itemList : Dropdown[] = [];
	itemDetail : any = [];


	serialErrorMessage : string;
	codeErrorMessage : string;

	successMessage : string;
	errorMessage : string;
	totalItems : any;
	totalQuantity : any;




	constructor(private fb: FormBuilder,
				private _commonViewService : CommonViewService,
				private _itemService : ItemService)
				//private _releasingService : ReleasingService
	{

		this.createForm();
		this.load();
		this.totalItems = 0;
		this.totalQuantity = 0;


	}

	getItemDetailsWithSerial() {
		if (this.serialNumber != null) {

			if (this.serialNumber == "") {
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
                            const control = this.newForm.controls['SoldItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.id );
                            let newQuantity = parseInt(itemDetails.controls.Quantity.value) + 1;
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
                        this.errorMessage = null;
                        this.successMessage = null;
                        this.serialErrorMessage = null; 
                        this.codeErrorMessage = null; 
					}
					else {
                        this.serialErrorMessage = "Serial Number do not Exist.";
                    }
                    this.serialNumber = "";
				},
                error =>{
                    this.errorMessage = this._commonViewService.getErrors(error);
                    this.successMessage = null;
                    this.serialErrorMessage = null; 
                    this.codeErrorMessage = null;  
                });
		}
	}

	getItemDetailsWithCode() {

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
                            const control = this.newForm.controls['SoldItems']['controls'];

                            let itemDetails = control.find(x => x.controls.ItemId.value == item.id );
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
                    $("#itemCode").val("");
                    $("#select2-itemCode-container").text("")
                    this.errorMessage = null;
                    this.successMessage = null;
                    this.serialErrorMessage = null; 
                    this.codeErrorMessage = null; 
				}); 
		}
 

        
	}



	onSubmit(){
		 let formData = this.newForm.value;
         console.log(formData);


        /* this._releasingService.newSalesRecord(formData,"sales")
         .subscribe(SuccessCode =>{
         	this.successMessage = "Sales successfully registered";

         },
         error =>{
         	this.errorMessage = this._commonViewService.getErrors(error);
         });*/

	}


	 addNewRow(data: any) {
        // control refers to your formarray
        const control = <FormArray>this.newForm.controls['SoldItems'];

        let newItem = this.fb.group({
                ItemId : new FormControl(data["id"],Validators.required),
                Quantity : new FormControl(1,[Validators.required]),
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
			TORNumber : new FormControl('',Validators.required),
			releaseDate : new FormControl('',Validators.required),
			SoldItems : this.fb.array([])							
		});
	}


	load() {
			this._commonViewService.getCommonList("items").subscribe(dll => {this.itemList = dll;});
	}

	//clear error and success mesagge when input new data
	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
	}


}