import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { OrderRequestService } from '@services/order_request/order-request.service';
import { OrderRequest } from '@models/order_request/order-request.model';

import { ProductService } from '@services/product/product.service';
import { Product } from '@models/product/product.model';
declare var jquery:any;
declare var $ :any;

@Component({
    selector: 'app-orl-details',
    templateUrl: 'orl-details.html'
})

export class OrderRequestListDetailsComponent implements OnChanges {

	@Input() id: number;
	@Input() rItems: any;
	@Input() updateRequestForm: FormGroup;
	@Input() orderRequest : OrderRequest;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
	itemDetail: any =[];
	@Output() statusMessage : EventEmitter<String> = new EventEmitter<String>();

	ngOnChanges(changes : SimpleChanges)
	{
		this.itemDetail = [];




		
		

		this.addItems(this.rItems.map(r => this.addItems(r)));

          
	}


	addItems(data: any){
		if(data.id != null)
		{
		const control = <FormArray>this.updateRequestForm.controls['requestedItems'];
		let newItemRow = this.fb.group({

			"id":[data["id"],Validators.required],
			"stInventoryId":[data["stInventoryId"],Validators.required],
			"itemId" : [data["itemId"],Validators.required],
			"approvedQuantity":[data["approvedQuantity"],Validators.required],
			"approvedRemarks": [data["approvedRemarks"],Validators.required]




		})
		this.itemDetail.push(data);
		control.push(newItemRow);
		}


	}


	onSubmit(data : any){
		let formData = this.updateRequestForm.value;
		console.log(formData);


		this._orderRequestService.updateRequestForm(this.id,formData)
		.subscribe(SuccessCode=>{
			console.log("success");
			this.updatePage.emit("loadPageRecord");
			this.statusMessage.emit("Request has been Approved and Updated");
			$("#details_modal").modal("hide");
		},
		error =>{
			console.log("error");
		});






	}

	   get RequestedItems(): FormArray{
        return this.updateRequestForm.get('requestedItems') as FormArray;
    }

    constructor(private fb: FormBuilder,
    	private _orderRequestService: OrderRequestService,
    	private _productService : ProductService) {
        
    }
}
