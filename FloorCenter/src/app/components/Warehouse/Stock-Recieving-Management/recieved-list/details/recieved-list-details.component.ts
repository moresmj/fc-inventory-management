import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { ProductService } from '@services/product/product.service';
import { Product } from '@models/product/product.model';

import { WarehouseInventoriesService } from '@services/warehouse/warehouse-inventories.service';
import { WarehouseInventories } from '@models/warehouse/Warehouse-inventories.model';
declare var jquery:any;
declare var $ :any;
@Component({
    selector: 'app-received-list-details',
    templateUrl: './recieved-list-details.html'
})

export class RecievedListDetailsComponent implements OnChanges {


	@Input()id:number;
	@Input()inventoryItem : any;
	@Input()updateInventoryForm: FormGroup;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
	@Output() statusMessage : EventEmitter<String> = new EventEmitter<String>();
	inventoryDetail: any = [];
	prodList : any =[];

    constructor(private fb: FormBuilder,
    	private _productService : ProductService,
    	private _warehouseInvetoryService : WarehouseInventoriesService,) {

    	 this._productService.getList().subscribe(
    	 	p =>{
    	 		this.prodList = p;
    	 	});
        
    }

	ngOnChanges(changes : SimpleChanges)
	{
		this.inventoryDetail = [];




		
		

		this.addItems(this.inventoryItem.deliveredItems.map(r => this.addItems(r)));

          
	}


	addItems(data: any){
		if(data.id != null)
		{
		const control = <FormArray>this.updateInventoryForm.controls['deliveredItems'];
		let newItemRow = this.fb.group({

			"id":[data["id"],Validators.required],
			"whInventoryId":[data["whInventoryId"],Validators.required],
			"quantity" : [data["quantity"],Validators.required],
			"remarks":[data["remarks"],Validators.required],
			"dateCreated": [data["dateCreated"],Validators.required],
			"dateUpdated" :[data["dateCreated"]]




		})

		let item = this.prodList.find(x => x.id == data.id);
		this.inventoryDetail.push(data);
		let index = this.inventoryDetail.length - 1;
		this.inventoryDetail[index]["code"]  = item.code;
		this.inventoryDetail[index]["itemName"]  = item.name;
		this.inventoryDetail[index]["size"]  = item.size.name;
		this.inventoryDetail[index]["tonality"]  = item.tonality;
		
	

		control.push(newItemRow);
		}


	}

	onSubmit(){
		let formData = this.updateInventoryForm.value;
		console.log(formData);


		this._warehouseInvetoryService.updateRecord(this.id,formData)
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



	   get deliveredItems(): FormArray{
        return this.updateInventoryForm.get('deliveredItems') as FormArray;
    }

    
}