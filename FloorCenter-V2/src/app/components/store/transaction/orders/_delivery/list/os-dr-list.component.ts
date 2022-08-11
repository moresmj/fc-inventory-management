import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';
import { del } from 'selenium-webdriver/http';





@Component({

	selector: 'app-os-dr-list',
	templateUrl: 'os-dr-list.html'
})



export class OrderStockDeliveryRequestListComponent implements OnInit{

orderStockDetails: OrderStock;
showRoomAddForm: FormGroup;
clientAddForm : FormGroup;
showRoomDeliveryList: any;
showRoomDeliveryItems : any;
clientDetails : any;
orisRequired : boolean;
drLabel : any;


		constructor(
		private _orderStockService : OrderStockService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private location: Location
        )
	    {
	   

	    }


	    ngOnInit(){

	
    	this.getDeliveryRequest();
	   

	    }


	    getDeliveryRequest(){

			    const id = +this.route.snapshot.paramMap.get('id');
			    const action = "/delivery/";

			    this._orderStockService.getRecordWithId(id,action)
			    .subscribe(order =>{
			    	this.orderStockDetails = order;
			    	console.log(order);
			    	this.getDeliveryRequestShowRoom();

			    });

	    }
l

	    getDeliveryRequestShowRoom(){
	    	const id = +this.route.snapshot.paramMap.get('id');
	    	let action = "";

	    	if((this.orderStockDetails.orderType == 2 || this.orderStockDetails.orderType == 3)&& this.orderStockDetails.deliveryType == 2 || this.orderStockDetails.deliveryType == 1)
	    	{
	    		 action = "/delivery/client/";
	    	}
	    	else{
	    		 action = "/delivery/showroom/";
	    	}
	    	

	    	this._orderStockService.getRecordWithId(id,action)
	    	.subscribe(order => {
	    		this.showRoomDeliveryList = order;

	    		// Table header is set on the API.
	    		this.orderStockDetails["tableHeader"] = order["tableHeader"];


	    		 for(let i = 0; i < this.showRoomDeliveryList.deliveries.length; i++)
				{

					if(this.showRoomDeliveryList.deliveries[i].showroomDeliveries != null)
					{

						for(let x = 0; x < this.showRoomDeliveryList.deliveries[i].showroomDeliveries.length; x++)
				            {	
								if(this.showRoomDeliveryList.deliveries[i].showroomDeliveries[x].releaseStatus == 2)
					            {
										this.showRoomDeliveryList.deliveries[i]["status"] = "Pending"
										break;
								}
								else if(this.showRoomDeliveryList.deliveries[i].showroomDeliveries[x].releaseStatus == 3){
										this.showRoomDeliveryList.deliveries[i]["status"] = "Waiting"
										break;
								}
								else
								{
									this.showRoomDeliveryList.deliveries[i]["status"] = "Released"
									break;
								}
							}

					}
					else{

							for(let x = 0; x < this.showRoomDeliveryList.deliveries[i].clientDeliveries.length; x++)
				            {	
								if(this.showRoomDeliveryList.deliveries[i].clientDeliveries[x].releaseStatus == 2)
					            {
										this.showRoomDeliveryList.deliveries[i]["status"] = "Pending"
										break;
								}
								else if(this.showRoomDeliveryList.deliveries[i].clientDeliveries[x].releaseStatus == 3){
									this.showRoomDeliveryList.deliveries[i]["status"] = "Waiting"
									break;
								}
								else
								{
									this.showRoomDeliveryList.deliveries[i]["status"] = "Released"
									break;
								}
							}


					}


		            
				}
	    		
	    

	    	});
	  


	    }


		onBtnViewDetailClick(deliveryType: number,orderType: number,data : any)
		{

			if((orderType == 2 || orderType == 3) /*&& deliveryType == 2*/)
			{
				if(deliveryType == 3)
				{
					this.showRoomDeliveryItems = data.showroomDeliveries;
				}
				else{
					this.showRoomDeliveryItems = data.clientDeliveries;
				}
				

			}
			else{
				this.showRoomDeliveryItems = data.showroomDeliveries;

			}

		

			
		}


	     onBtnAddClick(data : any): void{

	     	this.orderStockDetails = data;
	     	console.log(data);


	     	//if client dlivery

	     	if((data.orderType == 2 || data.orderType == 3) && data.deliveryType == 2 || data.deliveryType == 1)
	     	{

	     	this.clientDetails = this.orderStockDetails;

	     	if(data.orderType == 2)
	     	{

	     		if(data.deliveryType == 1)
	     		{

	     			this.clientAddForm = this.fb.group({
	     	


		           
		            Id : new FormControl(data.id),
		          
					SINumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),								
					ORNumber : new FormControl(''),								
					DRNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),								
					PreferredTime : new FormControl('',),							
		            ClientName : new FormControl(data.clientName),
		            address1 : new FormControl(data.address1),
		            address2 : new FormControl(data.address2),
		            address3 : new FormControl(data.address3),
		            ContactNumber: new FormControl(data.contactNumber),	
		            Remarks :	new FormControl(''),											
		            DeliveryDate : new FormControl('',Validators.required),
		            ClientDeliveries : this.fb.array([])




		        	});
	     		}
	     		else{

	     			this.clientAddForm = this.fb.group({
	     	


		           
		            Id : new FormControl(data.id),
		          
					SINumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),								
					ORNumber : new FormControl(''),								
					DRNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),								
					PreferredTime : new FormControl('',),							
		            ClientName : new FormControl(data.clientName,Validators.required),
		            address1 : new FormControl(data.address1,Validators.required),
		            address2 : new FormControl(data.address2),
		            address3 : new FormControl(data.address3),
		            ContactNumber: new FormControl(data.contactNumber),	
		            Remarks :	new FormControl(''),											
		            DeliveryDate : new FormControl('',Validators.required),
		            ClientDeliveries : this.fb.array([])




		        	});
	     		}


	     

	     		
	     	}
	     	else
	     	{
	     		this.clientAddForm = this.fb.group({
           
	            Id : new FormControl(data.id),
	          
				SINumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),								
				ORNumber : new FormControl(''),								
				DRNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),								
				PreferredTime : new FormControl('',),							
	            ClientName : new FormControl(data.clientName,Validators.required),
	            address1 : new FormControl(data.address1,Validators.required),
	            address2 : new FormControl(data.address2),
	            address3 : new FormControl(data.address3),
	            ContactNumber: new FormControl(data.contactNumber),	
	            Remarks :	new FormControl(''),											
	            DeliveryDate : new FormControl('',Validators.required),
	            ClientDeliveries : this.fb.array([])

	        	});
			}

	     	

        	 for(let i = 0; i < data["orderedItems"].length; i++)
				   {

					   	const control = <FormArray>this.clientAddForm.controls['ClientDeliveries'];

						   let item = data["orderedItems"][i];

						   console.log(this.getRemainingQty(item));
						   var remainingQuantity = this.getRemainingQty(item)


					   	let newItem = this.fb.group({
					   	STOrderId : new FormControl(item["stOrderId"]),						
						STOrderDetailId	: new FormControl(item["id"]),													
					   	itemId : new FormControl(item["itemId"]),
						Quantity : new FormControl(remainingQuantity,Validators.required),
						isTonalityAny: new FormControl(item["isTonalityAny"])



					   	})

					   	control.push(newItem);


				   }

				if(data.paymentMode != 10)
	     		{
	     			// this.clientAddForm.get('ORNumber').setValidators([Validators.required, Validators.maxLength(50)]);
	     			this.orisRequired = true;
	     		}else{
	     			this.orisRequired = false;
	     		}

	     		this.clientAddForm.addControl('paymentMode', new FormControl(data.paymentMode)); 


	     	}
	     	 	//if showroom dlivery
	     	else{

				     	this.showRoomAddForm = this.fb.group({


			           
			            Id : new FormControl(data.id),
			            //DRNumber : new FormControl('',Validators.required),
			            DeliveryDate : new FormControl('',Validators.required),
						TransactionNo : new FormControl(data.transactionNo),
						Remarks: new FormControl(),
			            ShowroomDeliveries : this.fb.array([])




			        	});

			        	   for(let i = 0; i < data["orderedItems"].length; i++)
							{

									   	const control = <FormArray>this.showRoomAddForm.controls['ShowroomDeliveries'];

										   let item = this.showRoomDeliveryList["orderedItems"][i];
										   var remainingQuantity = this.getRemainingQty(item)
										   console.log(remainingQuantity);
							




									   	let newItem = this.fb.group({
									   	STOrderId : new FormControl(item["stOrderId"]),						
										STOrderDetailId: new FormControl(item["id"]),
									   	itemId : new FormControl(item["itemId"]),
										quantity : new FormControl(remainingQuantity,Validators.required),
										isTonalityAny: new FormControl(item["isTonalityAny"])

									   	})

									   	control.push(newItem);


							}

			if (data.deliveryType == 3) {
				this.showRoomAddForm.addControl('DRNumber', new FormControl(data.whdrNumber));
				if(data.orNumber)
				{
					this.drLabel = "TOR No.";
				}else if(data.orderType == 3)
				{
					this.drLabel = "DR No.";
				}
				else{
					this.drLabel = "Warehouse DR No.";
				}
				
			}

	     	}


	}
	


	getRemainingQty(item : any)
	{

		var remainingQuantity = 0;
								
		//getting deliveries that is not auto generated for receiving
		var setForDeliveryQty = this.showRoomDeliveryList.deliveries.filter(p => p.isRemainingForReceivingDelivery == false)

		//getting each delivery quantity
		setForDeliveryQty.map(p => 
		 {
			 //checks if what delivery will get the quantity
			 var deliveries = p.showroomDeliveries != null ? p.showroomDeliveries.filter(x => x.itemId == item.itemId) : p.clientDeliveries.filter(x => x.itemId == item.itemId);
	
			 deliveries.map(c =>{
				 remainingQuantity += c.quantity;
			 });
   
   
		 });

   		return remainingQuantity = item["approvedQuantity"] - remainingQuantity;
	}






}