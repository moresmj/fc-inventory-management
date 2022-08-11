import { Component, AfterViewInit,OnInit,Input,ViewChild,ViewChildren} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import{ RequestService} from '@services/request.service';

import {Location} from '@angular/common';

import { CommonViewService } from '@services/common/common-view.service';
import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { ReceiveItems } from '@models/receive-items/receive-items.model';
import { Dropdown } from '@models/common/dropdown.model';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
declare var jquery:any;
declare var $:any;

import { AppConstants } from '@common/app-constants/app-constants';

@Component({

	selector:'app-ri-fo-registration',
	templateUrl: 'ri-fo-registration.html'

})


export class ReceiveItemsFromOrdersRegistrationComponent implements AfterViewInit, OnInit {
     template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'
    @ViewChildren('serialInput') serialInput;
    serialNumber : string;
    code: string;
    itemDetail: any = [];

    itemList : any = [];
    useridList : any = [];


    stockRegistrationForm: FormGroup; 
    errorMessage  : any;
    statusMessage: string;
    codeMessage : string;
    codeErrorMessage : string;
    Update : any;
    statusMessageItem: string;
    serialErrorMessage : string;

    currentDate: any = new Date().toISOString().substring(0,10);

    
    constructor(private fb: FormBuilder,
        private _itemService: ItemService,
        private _receiveItemsService: ReceiveItemsService,
        private _commonViewService: CommonViewService,
        private spinnerService: Ng4LoadingSpinnerService,
        private _location: Location,
        private _requestService : RequestService) { 
         this.loadDropDown();
    //  this.stockRegistrationForm = this.fb.group({

    //         PONumber: new FormControl('',[Validators.required, Validators.maxLength(50)]),
    //         DRNumber:new FormControl('',[Validators.required, Validators.maxLength(50)]),
    //         PODate:new FormControl(this.currentDate,Validators.required),
    //         DRDate:new FormControl(this.currentDate,Validators.required),
    //         ReceivedDate:new FormControl(this.currentDate,Validators.required),
    //         Remarks:new FormControl(''),
    //         CheckedBy:new FormControl('',Validators.required),
    //         SerialNumber:new FormControl(''),
    //         ReceivedItems: this.fb.array([])

               

    //     });
        this.createForm();

        this._requestService.action = "transactions/receiveitems/fromorders";


 }
onBtnViewDetailClick()
{
    this.statusMessage = null;
    this.errorMessage = null;
    this.Update = "new";
    this.statusMessageItem = null;

}

    reloadRecord(event : any) {

       this.statusMessageItem = event["statusMessage"];

   this.loadDropDown();
  } 

 private loadDropDown(): void{
    this._commonViewService.getCommonList("users")
                            .subscribe(ddl => {
                                 this.useridList = ddl;
                                this.useridList = this.useridList.filter(x => x.assignment == 2);
                            });
  
 this.spinnerService.show();
    this._commonViewService.getCommonList("items/itemfordropdown").subscribe(dll => {
        this.itemList = dll;
        this.spinnerService.hide();
    });

 }

    ngOnInit(){

    }

     getItemDetailsWithCode() {

      let id = $("#itemCode").val();

      if (id != '' && id.trim().length != 0) {

         let detail = this.itemList.find(x => x.id == id);
      
            if (detail != undefined) {
              let item = this.itemDetail.find(x => x.id == detail.id);
              if (item === undefined) {
                this.itemDetail.push(detail);
                this.addNewRow(detail,'');
                this.serialErrorMessage = null;
              }
              else {
                const control = this.stockRegistrationForm.controls['ReceivedItems']['controls'];

                let itemDetails = control.find(x => x.controls.ItemId.value == item.id);
                let newQuantity = parseInt(itemDetails.controls.Quantity.value) + 1;
                itemDetails.controls.Quantity.setValue(newQuantity);

                this.codeErrorMessage = null;
              }
            }
            else {
              this.codeErrorMessage = "Item Code do not Exist.";
            }
            this.clearData();
       
      }
      else{
        this.clearData();
        this.codeErrorMessage = "Please select Item";
      }

    }


    AddItemToList(event : any){
        this.code = event;
        this.getItemDetailsWithSerial();

    }

    setMessage(event){
        this.statusMessageItem = event;
    }


    getItemDetailsWithSerial() {
      if (this.serialNumber != null) {

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
              if (item === undefined) {
                this.itemDetail.push(detail);
                this.addNewRow(detail,qty);
                this.serialErrorMessage = null;
              }
              else {
                const control = this.stockRegistrationForm.controls['ReceivedItems']['controls'];

                let itemDetails = control.find(x => x.controls.ItemId.value == item.id);
                let newQuantity = parseInt(itemDetails.controls.Quantity.value) + parseInt(qty);
                itemDetails.controls.Quantity.setValue(newQuantity);


                this.serialErrorMessage = null;
              }
              
            }
            else {
                this.clearData();
                return this.serialErrorMessage = "Serial Number do not Exist.";
            }
            this.serialNumber = "";
            this.clearData();
    
      }
    }
       


    onChange(ch : any){


            this.codeMessage = null;
            this.statusMessageItem = null;
            this.errorMessage = null;
            this.statusMessage = null;
            this.serialErrorMessage = null;

    }



    addNewRow(data: any,qty :any){

            const control = <FormArray>this.stockRegistrationForm.controls['ReceivedItems'];


            if(qty != "")
            {
                qty =  parseInt(qty);
            }
            else{
                qty = 1;
            }


            let newItemRow = this.fb.group({
            ItemId:[data["id"]],
            Quantity: [qty,Validators.required],
            ReservedQuantity: [""],
            Remarks:[""]
          })


          control.push(newItemRow);
    }

    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.stockRegistrationForm.controls['ReceivedItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get ReceivedItems(): FormArray{
        return this.stockRegistrationForm.get('ReceivedItems') as FormArray;
    }






    onSubmit(data : any) {
        this._requestService.action = "transactions/receiveitems/fromorders";
        let formData = this.stockRegistrationForm.value;
        console.log(formData);

     
        this._requestService.newRecord(formData)
        .subscribe(successCode =>{
           
            this.errorMessage = null;
            this.statusMessage = "Stock Has been registered";
            this.itemDetail = [];
            this.codeMessage = null;
            this.reset();


            $("#saveModal").modal("hide");
      
        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.statusMessage = null;
            $("#saveModal").modal("hide");
        });


    }


    createForm(){
         this.stockRegistrationForm = this.fb.group({

            PONumber: new FormControl('',Validators.required),
            DRNumber:new FormControl('',Validators.required),
            PODate:new FormControl(this.currentDate,Validators.required),
            DRDate:new FormControl(this.currentDate,Validators.required),
            ReceivedDate:new FormControl(this.currentDate,Validators.required),
            Remarks:new FormControl(''),
            userId : new FormControl('',Validators.required),
            SerialNumber:new FormControl(''),
            ReceivedItems: this.fb.array([])

               

        });
    }
    reset(){
        this.createForm();
    }

      goback(){
        this._location.back();
    }


     onSelectCode()
    {
        let id = $("#itemCode").val();
 

        if (id != '' && id.trim().length != 0){

            let item = this.itemList.find(x => x.id == id);
            console.log(item);
            $("#serial").val(item.serialNumber.toString());
            this.serialNumber = item.serialNumber.toString();

            this.codeErrorMessage = null;
     


        }
    }

    clearData(){
        this.code ="";
        this.serialNumber = "";
        $("#itemCode").val("");
        $("#select2-itemCode-container").text("");
        this.codeErrorMessage = null;
        this.codeMessage = null;
        this.serialErrorMessage = null;
    }


    async ngAfterViewInit(){
        this.serialInput.first.nativeElement.focus();
       /* $(document).ready(function(){
            $("#poDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#drDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#rDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns:true
            });
        });*/
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