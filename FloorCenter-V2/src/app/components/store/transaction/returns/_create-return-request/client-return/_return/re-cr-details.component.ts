import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';
import { ApiBaseService } from '@services/api-base.service';
import { RequestService } from '@services/request.service';
import { OrderStock } from '@models/order-stock/order-stock.model';
import { AppConstants } from '@common/app-constants/app-constants';





@Component({

	selector: 'app-re-cr-details',
	templateUrl: './re-cr-details.html'
})



export class ClientReturnDetailsComponent implements OnInit{

	currentDate: any = new Date().toISOString().substring(0,10);
	returnDetails: any;
	updateForm: FormGroup;
	returnAddForm : FormGroup;
	returnedItems : any;
	returnDeliveryDetails : any;
	returnId : any;
	returnReasonList : any = [];
	clientReturnTypeList : any = [];

	showSaveBtn : boolean = true;
	successMessage : string;
	errorMessage : string;
	isItemRemarkRequired : any;
	isReasonRequired : any;
	isUpdated : boolean = false;
	isRequestPickup : boolean = false;


	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _apiBaseService : ApiBaseService,
			private route: ActivatedRoute,
			private location: Location,
			private _requestService : RequestService
			)
			{
				this._requestService.action = "returns/clientreturn/";
				this._apiBaseService.action = "returns/clientreturn/";
				this.returnId = route.snapshot.params['id'];
				this.load();
			}


			ngOnInit(){
					this.getDeliveryReturn();

			}

			createForm(id: any) {
				this.updateForm = this.fb.group({
					id : new FormControl(id),
					ClientReturnType: new FormControl("",Validators.required),
					pickUpDate : new FormControl(),
					ReturnDRNumber : new FormControl("",Validators.required),
					Remarks : new FormControl(""),
				    PurchasedItems: this.fb.array([]) // here
				});
			}



			getDeliveryReturn(){

			    const id = +this.route.snapshot.paramMap.get('id');
			    

			    this._apiBaseService.getRecordWithId(id)
			    .subscribe(returns =>{
			    	console.log(returns);
			    	this.returnDetails = returns;

			    	this.createForm(id);

			    	//  Record will always be 1
				for (let a = 0; a <  returns["soldItems"].length; a++ ) {


						let deliveries = returns["soldItems"][a] 

						const control = <FormArray>this.updateForm.controls['PurchasedItems'];
				        let newItemRow = this.fb.group({

					            id : new FormControl(deliveries["id"]),
					            STSalesId : new FormControl(deliveries["stSalesId"]),
					            ItemId : new FormControl(deliveries["itemId"]),
					            Quantity : new FormControl(0,Validators.required),
					            ReturnReason : new FormControl(),
					            Remarks : new FormControl()	                      
				        })
        				control.push(newItemRow);
					}



			    	//this.getDeliveryStatus();
			    	
			    	console.log(returns);
			    

			    });

	    	}


	 onSubmit(){
	 	let formData = this.updateForm.value;
	 	console.log(formData);


        this._requestService.updateRecord(this.returnId,formData)
            .subscribe(successCode =>{
                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.showSaveBtn = false;
                this.errorMessage = null;
                this.isUpdated = true; 
                this.getDeliveryReturn(); 
                // this.serialErrorMessage = null;  
            },
            error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
                this.successMessage = null;
                this.isUpdated = false; 
                // this.serialErrorMessage = null; 
            });	 
	 }


	get PurchasedItems(): FormArray{
        return this.updateForm.get('PurchasedItems') as FormArray;
    }

    load() {
    
        this._commonViewService.getCommonList("returnreason",true).subscribe(dll => { 
			this.returnReasonList = dll; 
			this.returnReasonList = this.returnReasonList.filter(x => x["value"] != 1);
			console.log(dll);   
        });
        this._commonViewService.getCommonList("clientreturntype",true).subscribe(dll => { 
			this.clientReturnTypeList = dll;
			   
        });

     


	}


	setDelivery(value : number) {

        let fields = ["pickUpDate"];
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() === "2") {
            	
                    this.updateForm.controls[fields[i]].setValidators([Validators.required]);
					this.updateForm.controls[fields[i]].updateValueAndValidity(); 
					this.isRequestPickup = true;  
            }
            else {

                    this.updateForm.controls[fields[i]].setValidators(null);
					this.updateForm.controls[fields[i]].updateValueAndValidity();
					this.isRequestPickup = false;
            }
        }
	}
	
	

    checkQuantity(value : any,index: number)
    {

    	let fields = ["ReturnReason"];
        const control = <FormArray>this.updateForm.controls['PurchasedItems'];
        
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() != "0" && value.length != 0) {
                    this.isReasonRequired = true;
                    control["controls"][index]["controls"][fields[i]].setValidators([Validators.required]);
                    control["controls"][index]["controls"][fields[i]].updateValueAndValidity();  
            }
            else {
                    this.isReasonRequired = false;
                    control["controls"][index]["controls"][fields[i]].setValidators(null);
                    control["controls"][index]["controls"][fields[i]].updateValueAndValidity();  
            }
        }


    }


    setReason(value : number,index: number)
    {

    	let fields = ["Remarks"];
        const control = <FormArray>this.updateForm.controls['PurchasedItems'];
        
        for(let i = 0; i < fields.length; i++)
        {
            if (value.toString() === "5"  ) {
                    this.isItemRemarkRequired = true;
                    control["controls"][index]["controls"][fields[i]].setValidators([Validators.required]);
                    control["controls"][index]["controls"][fields[i]].updateValueAndValidity();  
            }
            else {
                    this.isItemRemarkRequired = false;
                    control["controls"][index]["controls"][fields[i]].setValidators(null);
                    control["controls"][index]["controls"][fields[i]].updateValueAndValidity();  
            }
        }


    }




	 onChange(ch : any){
			this.successMessage = null;
			this.errorMessage = null;
			this.showSaveBtn = true;

    }


	    
	   

	onCancel() {
    	$("#saveModal").modal("hide");
    }




}