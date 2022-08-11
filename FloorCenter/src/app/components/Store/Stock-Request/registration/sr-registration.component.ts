import { Component, ElementRef, ViewChild } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';
import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { ShowErrorsComponent } from '@components/show-errors/show-errors.component';
import { Router } from '@angular/router';

declare var jquery:any;
declare var $:any;

@Component({
	selector : 'app-stock-reuqest-registration',
	templateUrl : './sr-registration.html'
})

export class StockRequestRegistrationComponent {

 	serialNumber : string;
    @ViewChild('serialNumberInput') serialNumberElement:ElementRef;

    itemDetail : any = [];

    transactionTypesList : Dropdown[] = [];
    warehouseList : Dropdown[] = [];

    registrationForm : FormGroup;

    currentDate: any = new Date().toISOString().substring(0,10);
 

    serialMessage : string;
    successMessage : string;
    errorMessage  : any;
    router : Router;

    constructor(
            private fb : FormBuilder, 
            private _commonViewService : CommonViewService,
            private _itemService : ItemService,
            private _router : Router
        )
    {
        this.load();
        this.registrationForm = this.fb.group({
            WarehouseId : new FormControl(''),
            TransactionType : new FormControl('',Validators.required),
            PODate : new FormControl(this.currentDate),
            PONumber : new FormControl(''),
            Remarks : new FormControl(''),
            RequestedItems: this.fb.array([]) // here
        });
    }

    load() {

        this._commonViewService.getCommonList("warehouses").subscribe(dll => {this.warehouseList = dll;});
        this._commonViewService.getCommonList("transactiontypes",true).subscribe(dll => 
            {
                this.transactionTypesList = dll;
                this.transactionTypesList = this.transactionTypesList.filter(x =>  x["value"] == 11 || x["value"] == 12  ||  x["value"] == 15);
            });       
    }

    redirect() {
        this._router.navigateByUrl('Store/stock_list');
    }

   transactionCondition(value : number) {

        let validator = null;
        if ((value == 11) || (value == 15)) {
            validator = [Validators.required];
        }

        let controls = ["WarehouseId","PONumber","PODate"];

        controls.map(field => {
            this.registrationForm.controls[field].setValidators(validator);
            this.registrationForm.controls[field].updateValueAndValidity();
        });
    }

    getItemDetails() {
        if (this.serialNumber != '' && this.serialNumber.trim().length != 0) {                  
            this._itemService.getRecordDetailsWithCode(this.serialNumber)
                .subscribe(detail => {
                                if (detail[0] !== undefined) {
                                    let item = this.itemDetail.find(x => x.serialNumber == detail[0]["serialNumber"]);
                                    if(item === undefined) {
                                            this.itemDetail.push(detail[0]);
                                            this.addNewRow(detail[0]); 
                                            this.serialMessage = null;
                                            this.serialNumberElement.nativeElement.focus();
                                    }
                                    else{
                                        const control = this.registrationForm.controls['RequestedItems']['controls'];

                                        let itemDetails = control.find(x => x.controls.ItemId.value == item.id );
                                        let newQuantity = pareseitemDetails.controls.RequestedQuantity._value += 1;
                                        itemDetails.controls.RequestedQuantity.setValue(newQuantity);

                                        this.serialMessage = null;
                                    }
                                }
                                else{
                                    this.serialMessage = "Serial Number do not Exist.";
                                }
                                this.serialNumber = "";
                            }); 
        }                                                          
    }
    addNewRow(data: any) {
        // control refers to your formarray
        const control = <FormArray>this.registrationForm.controls['RequestedItems'];

        let newItem = this.fb.group({
                ItemId : new FormControl(data["id"],Validators.required),
                RequestedQuantity : new FormControl(1,Validators.required),
                RequestedRemarks : ['']
        })
        // add new formgroup
        control.push(newItem);
    }


    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.registrationForm.controls['RequestedItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get RequestedItems(): FormArray{
        return this.registrationForm.get('RequestedItems') as FormArray;
    }

    onSubmit(data : any) {
        let formData = this.registrationForm.value;
        console.log(formData);

        this._itemService.newRecord(formData)
        .subscribe(successCode =>{
            this.registrationForm.reset();
            this.itemDetail = [];
            this.registrationForm.controls['RequestedItems'] = this.fb.array([]);

            this.successMessage = "Records Succesfully Saved";
            this.errorMessage = null;      
            this.serialMessage = null;
            $("#saveModal").modal("hide");    
        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.successMessage = null;
            this.serialMessage = null;
            $("#saveModal").modal("hide");  
        });

    }


    cancelSave() {
        $("#saveModal").modal("hide");  
    }

}