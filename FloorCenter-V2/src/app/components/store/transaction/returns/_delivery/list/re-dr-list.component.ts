import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';
import { ApiBaseService } from '@services/api-base.service';
import { OrderStock } from '@models/order-stock/order-stock.model';





@Component({

	selector: 'app-re-dr-list',
	templateUrl: 're-dr-list.html'
})



export class ReturnDeliveryRequestListComponent implements OnInit{
currentDate: any = new Date().toISOString().substring(0,10);
returnDetails: any;
showRoomAddForm: FormGroup;
returnAddForm : FormGroup;
returnedItems : any;
returnDeliveryDetails : any;


	constructor(
			private fb : FormBuilder,
			private _commonViewService : CommonViewService,
			private _apiBaseService : ApiBaseService,
			private route: ActivatedRoute,
        	private location: Location
			)
			{
				this._apiBaseService.action = "returns/rtv/delivery";
			}


			ngOnInit(){
					this.getDeliveryReturn();

			}


			getDeliveryReturn(){

			    const id = +this.route.snapshot.paramMap.get('id');
			    

			    this._apiBaseService.getRecordWithId(id)
			    .subscribe(returns =>{
			    	this.returnDetails = returns;
			    	console.log(returns);
			    });

	    	}

	    	onBtnViewDetailClick(data : any)
	    	{
	    		console.log(data);
	    		this.returnDeliveryDetails = data.warehouseDeliveries;
	    		console.log(data.warehouseDeliveries);

	    	}


	    	onBtAddClick(data: any): void{
	    		this.returnedItems = data;



	    		this.returnAddForm = this.fb.group({

	    			Id	: new FormControl(data.id,Validators.required),								
					DRNumber	: new FormControl('',[Validators.required, Validators.maxLength(50)]),								
					DeliveryDate: new FormControl(this.currentDate,Validators.required),	
					Remarks : new FormControl(''),														
					
					WarehouseDeliveries	: this.fb.array([])										



	    		});

			    		for(let i = 0; i < data["purchasedItems"].length; i++)
						{

							 
							   	const control = <FormArray>this.returnAddForm.controls['WarehouseDeliveries'];

							   	let item = data["purchasedItems"][i];

							  let qty = item["goodQuantity"] + item["brokenQuantity"];


							   	let newItem = this.fb.group({
							   	
									STReturnId	: new FormControl(item["stReturnId"],Validators.required),								
									STPurchaseReturnId	: new FormControl(item["id"],Validators.required),								
									ItemId	: new FormControl(item["itemId"],Validators.required),								
									Quantity: new FormControl(qty,Validators.required),


							   	})

							   	control.push(newItem);


						 }



	    	}






}