import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';
import { ApiBaseService } from '@services/api-base.service';
import { OrderStock } from '@models/order-stock/order-stock.model';
import { AppConstants } from '@common/app-constants/app-constants';
import { RequestService } from '@services/request.service';





@Component({

	selector: 'app-ri-fr-details',
	templateUrl: 'ri-fr-details.html'
})



export class ReceiveReturnDetailsComponent implements OnInit{

	currentDate: any = new Date().toISOString().substring(0,10);
	returnDetails: any;
	updateForm: FormGroup;
	returnAddForm : FormGroup;
	returnedItems : any;
	returnDeliveryDetails : any;
	returnId : any;

	showSaveBtn : boolean = true;
	successMessage : string;
	errorMessage : string;


	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _apiBaseService : ApiBaseService,
			private route: ActivatedRoute,
			private location: Location,
			private _requestSerice : RequestService,
			)
			{
				this._apiBaseService.action = "transactions/receiveitems/returns/";
				this._requestSerice.action = "transactions/receiveitems/returns/";
				this.returnId = route.snapshot.params['id'];
			}


			ngOnInit(){
					this.getDeliveryReturn();

			}

			createForm(id: any, returns: any) {
				this.updateForm = this.fb.group({
					id : new FormControl(id),
					transactionNo : new FormControl(returns.transactionNo),
				    deliveredItems : this.fb.array([]) // here
				});
			}



			getDeliveryReturn(){

			    const id = +this.route.snapshot.paramMap.get('id');
			    

			    this._apiBaseService.getRecordWithId(id)
			    .subscribe(returns =>{
					this.returnDetails = returns;
					console.log(returns);

			    	this.createForm(id, returns);

			    	//  Record will always be 1
				for (let a = 0; a <  returns["deliveries"].length; a++ ) {


						let deliveries = returns["deliveries"][a] 

						const control = <FormArray>this.updateForm.controls['deliveredItems'];
				        let newItemRow = this.fb.group({

					            id : new FormControl(deliveries["id"]),
					            whDeliveryId : new FormControl(deliveries["whDeliveryId"]),
					            stPurchaseReturnId: new FormControl(deliveries["stPurchaseReturnId"]),
					            itemId : new FormControl(deliveries["itemId"]),
					            goodQuantity : new FormControl(0,Validators.required),
					            brokenQuantity : new FormControl(0,Validators.required),							            	  	 
					            receivedRemarks : new FormControl()	                      
				        })
        				control.push(newItemRow);
					}



			    	this.getDeliveryStatus();
			    	
			    	console.log(returns);
			    

			    });

	    	}


	 onSubmit(){
	 	let formData = this.updateForm.value;
	 	console.log(formData);


        this._requestSerice.updateRecord(this.returnId,formData)
            .subscribe(successCode =>{
                this.successMessage = AppConstants.recordSaveSuccessMessage;
                this.showSaveBtn = false;
                this.errorMessage = null;      
                // this.serialErrorMessage = null;  
            },
            error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
                this.successMessage = null;
                // this.serialErrorMessage = null; 
            });	 	
	 }


	get deliveredItems(): FormArray{
        return this.updateForm.get('deliveredItems') as FormArray;
    }

	    	getDeliveryStatus()
	    	{
	    		for(let i = 0; i < this.returnDetails.deliveries.length; i++)
				{

					if(this.returnDetails.deliveries[i].warehouseDeliveries != null)
					{
						for(let x = 0; x < this.returnDetails.deliveries[i].warehouseDeliveries.length; x++)
						{
								if(this.returnDetails.deliveries[i].warehouseDeliveries[x].releaseStatus == 3)
						            {
											this.returnDetails.deliveries[i]["status"] = "Pending"
											break;


									}
									else
									{
											this.returnDetails.deliveries[i]["status"] = "Released"
											

									}

						}

					}
				}
	    	}


	    
	   

	onCancel() {
    	$("#saveModal").modal('hide');
    }




}