import { Component, AfterViewInit,OnInit,Input,ViewChild,ViewChildren} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ProductService } from '@services/product/product.service';
import { Product } from '@models/product/product.model';
import { Dropdown } from '@models/common/dropdown.model';
declare var jquery:any;
declare var $:any;

@Component({
    selector: 'app-stock-registation',
    templateUrl: './stock-registration.html'
})

export class StockRegistrationComponent implements AfterViewInit, OnInit {
    @ViewChildren('serialInput') serialInput;
    code: string;
    itemDetail: any = [];

    useridList : Dropdown[] = [];


    stockRegistrationForm: FormGroup; 
    errorMessage  : any;
    statusMessage: string;
    codeMessage : string;

    currentDate: any = new Date().toISOString().substring(0,10);

    
    constructor(private fb: FormBuilder,
        private _productService: ProductService,
        private _commonViewService: CommonViewService) { 
         this.loadDropDown();
     this.stockRegistrationForm = this.fb.group({

            PONumber: new FormControl('',Validators.required),
            DRNumber:new FormControl('',Validators.required),
            PODate:new FormControl(this.currentDate,Validators.required),
            DRDate:new FormControl(this.currentDate,Validators.required),
            ReceivedDate:new FormControl(this.currentDate,Validators.required),
            Remarks:new FormControl(''),
            CheckedBy:new FormControl('',Validators.required),
            SerialNumber:new FormControl(''),
            DeliveredItems: this.fb.array([])

               

        });


 }


 private loadDropDown(): void{
    this._commonViewService.getCommonList("users")
                            .subscribe(ddl => {this.useridList = ddl; });
 }

    ngOnInit(){

    }


    getItemDetails(){

        if(this.code != "" && this.code.trim().length != 0)
        {
             this._productService.getRecordDetailsWithSerial(this.code)
            .subscribe(detail => {
                            if (detail != null) {
                                 if (detail[0] !== undefined) {
                                let item = this.itemDetail.find(x => x.serialNumber == detail[0]["serialNumber"]);
                                if(item === undefined) {
                                        this.itemDetail.push(detail[0]);
                                        this.addNewRow(detail[0]); 
                                        this.codeMessage = null;
                                       
                                    }
                                    else{
                                          const control = this.stockRegistrationForm.controls['DeliveredItems']['controls'];

                                         let itemCount = control.find(x => x.controls.ItemId.value == item.id );
                                         let _quantity =  itemCount.controls.Quantity._value += 1;
                                         itemCount.controls.Quantity.setValue(_quantity);

                                        this.codeMessage = null;

                                    }
                                }
                                 else{
                                    this.codeMessage = "Serial Number do not Exist.";
                                    this.statusMessage = null;
                                }

                                }

                                this.code ="";
                            }); 
         
        }
        else
        {
            this.code ="";

        return this.codeMessage = "Please enter Serial Number";


        }
       
    }


    addNewRow(data: any){

            const control = <FormArray>this.stockRegistrationForm.controls['DeliveredItems'];
            let newItemRow = this.fb.group({
            ItemId:[data["id"]],
            Quantity: [1]
          })
          control.push(newItemRow);
    }

    deleteRow(index: number) {
        // control refers to your formarray
        const control = <FormArray>this.stockRegistrationForm.controls['DeliveredItems'];
        // remove the chosen row
        this.itemDetail.splice(index,1);
        control.removeAt(index);
    }

    get DeliveredItems(): FormArray{
        return this.stockRegistrationForm.get('DeliveredItems') as FormArray;
    }






    onSubmit(data : any) {
        let formData = this.stockRegistrationForm.value;
        console.log(formData);

     
        this._productService.addRecordStock(formData)
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
            CheckedBy:new FormControl('',Validators.required),
            SerialNumber:new FormControl(''),
            DeliveredItems: this.fb.array([])

               

        });
    }
    reset(){
        this.createForm();
    }


    async ngAfterViewInit(){
        this.serialInput.first.nativeElement.focus();
        $(document).ready(function(){
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
        });
    }
}