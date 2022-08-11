import { Component, AfterViewInit, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ProductService } from '@services/product/product.service';
declare var $jquery: any;
declare var $: any;

@Component({
    selector: 'app-sl-registration',
    templateUrl: './sl-registration.html'
})

export class SalesReleasingRegistrationComponent implements AfterViewInit, OnInit {
    code: string;
    itemDetail: any = [];

    salesReleaseRegistrationForm: FormGroup;
    errorMessage: any;
    statusMessage: string;

    constructor(private _formBuilder: FormBuilder, 
        private _productService: ProductService,
        private _commonViewService: CommonViewService) {

        this.salesReleaseRegistrationForm = this._formBuilder.group({
            PONumber: new FormControl('', Validators.required),
            CustomerName: new FormControl('', Validators.required),
            DeliveryAddress: new FormControl('', Validators.required),
            DeliveryAddress2: new FormControl(''),
            DeliveryAddress3: new FormControl(''),
            PODate: new FormControl('', Validators.required),
            ContactNumber: new FormControl(''),
            ContactNumber2: new FormControl(''),
            ContactNumber3: new FormControl(''),
            SalesAgent: new FormControl(''),
            Remarks: new FormControl(''),
            PaymentMethod: new FormControl(''),
            DeliveryOption: new FormControl(''),
            SalesReleasedItems: this._formBuilder.array([])
        });
    }


    ngOnInit(){

    }



    getItemDetails(data: any) {
        this._productService.getRecordDetailsWithSerial(data.currentTarget.value)
            .subscribe(detail => {
                if (detail[0] != undefined) {
                    let item = this.itemDetail.find(x => x.serialNumber  == detail[0]["serialNumber"]);
                    if (item === undefined) {
                        this.itemDetail.push(detail[0]);
                        this.addNewRow(detail[0]);
                    }
                }
            });
    }



    addNewRow(data: any) {
        const control = <FormArray>this.salesReleaseRegistrationForm.controls['SalesReleasedItems'];
        let newItemRow = this._formBuilder.group({
            ItemId: [data["id"]],
            Quantity: ['']
        })
        control.push(newItemRow);
    }



    deleteRow(index: number) {
        const control = <FormArray>this.salesReleaseRegistrationForm.controls["SalesReleasedItems"];
        this.itemDetail.splice(index, 1);
        control.removeAt(index);
    }



    get SalesReleasedItems(): FormArray {
        return this.salesReleaseRegistrationForm.get('SalesReleasedItems') as FormArray;
    }




    onSubmit(data: any) {
        let formData = this.salesReleaseRegistrationForm.value;
        console.log(formData);

        this._productService.addRecordStock(formData)
        .subscribe(successCode => {
            this.errorMessage = null;
            this.statusMessage = "Stock Has been Registered";
            this.reset();
            $("#saveModal").modal("hide");
        },
        error => {
            this.errorMessage = this._commonViewService.getErrors(error);
            $("#saveModal").modal("hide");
        });
    }




    reset() {
        this.createForm();
    }


    

    createForm(){
        this.salesReleaseRegistrationForm = this._formBuilder.group({
            PONumber: new FormControl('', Validators.required),
            CustomerName: new FormControl('', Validators.required),
            DeliveryAddress: new FormControl('', Validators.required),
            DeliveryAddress2: new FormControl(''),
            DeliveryAddress3: new FormControl(''),
            PODate: new FormControl('', Validators.required),
            ContactNumber: new FormControl(''),
            ContactNumber2: new FormControl(''),
            ContactNumber3: new FormControl(''),
            SalesAgent: new FormControl(''),
            Remarks: new FormControl(''),
            PaymentMethod: new FormControl(''),
            DeliveryOption: new FormControl(''),
            SalesReleasedItems: this._formBuilder.array([])
        });
   }



    async ngAfterViewInit() {
        $(document).ready(function(){
            $('#poDate').daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        })
    }
    
}