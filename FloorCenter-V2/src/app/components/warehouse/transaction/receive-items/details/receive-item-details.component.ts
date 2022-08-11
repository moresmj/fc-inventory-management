import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { ItemService } from '@services/item/item.service';
import { Item } from '@models/item/item.model';

import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { ReceiveItems } from '@models/receive-items/receive-items.model';

declare var jquery:any;
declare var $ :any;
@Component({

	selector:'app-receive-item-details',
	templateUrl: 'receive-item-details.html'

})


export class ReceiveItemDetailsComponent implements OnChanges {


	@Input()id:number;
	@Input()inventoryItem : any;
	@Input()updateInventoryForm: FormGroup;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
	@Output() statusMessage : EventEmitter<String> = new EventEmitter<String>();
	inventoryDetail: any = [];
	itemList : any =[];

    constructor(private fb: FormBuilder,
    	private _itemService : ItemService,
    	private _receiveItemsService : ReceiveItemsService){

   	 /*this._itemService.getList().subscribe(
    	 	p =>{
    	 		this.itemList = p;
    	 	});

        */
    }

	ngOnChanges(changes : SimpleChanges)
	{
		this.inventoryDetail = [];



		 
		

		this.addItems(this.inventoryItem.receivedItems.map(r => this.addItems(r)));



          
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

	
		this.inventoryDetail.push(data);
		let index = this.inventoryDetail.length - 1;
		this.inventoryDetail[index]["code"]  = data["item"].code;
		this.inventoryDetail[index]["itemName"]  = data["item"].name;
		this.inventoryDetail[index]["size"]  = data["item"].size.name;
		this.inventoryDetail[index]["tonality"]  = data["item"].tonality;
		
	

		control.push(newItemRow);
		}


	}

	onSubmit(){
		let formData = this.updateInventoryForm.value;
		console.log(formData);


		this._receiveItemsService.updateRecord(this.id,formData)
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