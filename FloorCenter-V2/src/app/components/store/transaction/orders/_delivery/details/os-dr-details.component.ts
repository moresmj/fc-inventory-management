import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';





@Component({

	selector: 'app-os-dr-details',
	templateUrl: 'os-dr-details.html'
})



export class OrderStockDeliveryRequestDetailsComponent implements OnInit{

	   @Input()showRoomDeliveryItems : any;


		orderStockDetails: OrderStock;
		showRoomAddForm: FormGroup;
		showRoomDeliveryList: any;
		showRoomDeliveries : any;


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
			    .subscribe(order => {
			    	this.orderStockDetails = order
			    	//this.getDeliveryRequestShowRoom();
			    });

	    }


	    getDeliveryRequestShowRoom(){
	    	const id = +this.route.snapshot.paramMap.get('id');
	    	let action = "";

	    	if((this.orderStockDetails.orderType == 2 || this.orderStockDetails.orderType == 3)&& this.orderStockDetails.deliveryType == 2)
	    	{
	    		 action = "/delivery/client/";
	    	}
	    	else
	    	{
	    		 action = "/delivery/showroom/";
	    	}
	    	
	    

	    	this._orderStockService.getRecordWithId(id,action)
	    	.subscribe(order => {
	    		this.showRoomDeliveryList = order;
	    		this.showRoomDeliveries = this.showRoomDeliveryList.deliveries;
	    	

	    	});
	  


	    }




	     onBtnAddClick(data : any): void{

	     	this.orderStockDetails = data;



	     	this.showRoomAddForm = this.fb.group({


           
            Id : new FormControl(data.id),
            DRNumber : new FormControl(),
            DeliveryDate : new FormControl(),
            ShowroomDeliveries : this.fb.array([])




        });

	   for(let i = 0; i < data["orderedItems"].length; i++)
	   {

	   	const control = <FormArray>this.showRoomAddForm.controls['ShowroomDeliveries'];

	   	let item = data["orderedItems"][i];


	   	let newItem = this.fb.group({
		STOrderDetailId: new FormControl(item["id"]),
	   	itemId : new FormControl(item["itemId"]),
	   	quantity : new FormControl(1)



	   	})

	   	control.push(newItem);


	   }

    }








}