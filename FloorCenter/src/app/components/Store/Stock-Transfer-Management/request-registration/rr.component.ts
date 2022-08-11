import { Component } from '@angular/core';

import { FormBuilder, FormArray, FormGroup, FormControl, Validators, NgModel } from '@angular/forms';
import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { ShowErrorsComponent } from '@components/show-errors/show-errors.component';

@Component({
    selector:'app-rr',
    templateUrl: './rr.html'
})

export class RequestRegistrationComponent {
  
    code : string;
    itemDetail : any = [];

    transactionTypesList : Dropdown[] = [];
    warehouseList : Dropdown[] = [];

    registrationForm : FormGroup;

    codeMessage : string;
    successMessage : string;
    errorMessage  : any;

    constructor(
            private fb : FormBuilder, 
            private _commonViewService : CommonViewService,
            private _itemService : ItemService
        )
    {
        this.load();
        this.registrationForm = this.fb.group({
            WarehouseId : new FormControl(''),
            TransactionType : new FormControl('',Validators.required),
            PODate : new FormControl(''),
            PONumber : new FormControl(''),
            Remarks : new FormControl(''),
            RequestedItems: this.fb.array([]) // here
        });
    }

    load() {
        this._commonViewService.getCommonList("warehouses").subscribe(dll => {this.warehouseList = dll;});
        this._commonViewService.getCommonList("transactiontypes",true).subscribe(dll => {this.transactionTypesList = dll;});       
    }

    delivOptCondition(value : number) {

        let validator = (value != 10) ? [Validators.required] : null;
        let controls = ["WarehouseId","PONumber","PODate"];

        controls.map(field => {
            this.registrationForm.controls[field].setValidators(validator);
            this.registrationForm.controls[field].updateValueAndValidity();
        });
    }

    getItemDetails() {

        this._itemService.getRecordDetailsWithCode(this.code)
            .subscribe(detail => {
                            if (detail[0] !== undefined) {
                                let item = this.itemDetail.find(x => x.code == detail[0]["code"]);
                                if(item === undefined) {
                                        this.itemDetail.push(detail[0]);
                                        this.addNewRow(detail[0]); 
                                        this.codeMessage = null;
                                }
                                else{
                                    this.codeMessage = "Item Code already Exist.";
                                }
                            }
                            else{
                                this.codeMessage = "Item Code do not Exist.";
                            }
                            this.code = "";
                        }); 
                                                        
    }
    addNewRow(data: any) {
        // control refers to your formarray
        const control = <FormArray>this.registrationForm.controls['RequestedItems'];

        let newItem = this.fb.group({
                ItemId : new FormControl(data["id"],Validators.required),
                RequestedQuantity : new FormControl('',Validators.required),
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
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
        });

    }
}