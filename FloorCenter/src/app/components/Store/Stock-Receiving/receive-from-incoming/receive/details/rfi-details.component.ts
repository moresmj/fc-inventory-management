import { Component, OnInit,ViewChild,ViewChildren} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

import { ProductService } from '@services/product/product.service';
import { StockService } from '@services/stock/stock.service';
import { Stock } from '@models/stock/stock.model';


import { Observable } from 'rxjs/Observable';

@Component({
	selector : 'app-receive-from-incoming-details',
	templateUrl : './rfi-details.html'
})

export class ReceiveFromIncomingDetailsComponent implements OnInit {
	@ViewChildren('serialInput') serialInput;
	code: string;
	
	transactionNumber: number;
	stock : Stock;
  	private sub: any;
  	 SRDetailsFrom: FormGroup;
  	 stockDetail: any = [];
  	 productDetail: any =[];

	constructor(
			private route: ActivatedRoute,
			private fb : FormBuilder,
    		private router: Router,
    		private _stockService : StockService,
    		private _productService : ProductService
		)  {

		this.transactionNumber = route.snapshot.params['transactionNumber'];
		console.log(this.transactionNumber);
		this.getStockDetails();

		this.SRDetailsFrom = this.fb.group({
			id: new FormControl(this.transactionNumber, Validators.required),
			receiveBy: new FormControl('',Validators.required),

			 itemsToBeDelivered: this.fb.array([])

		});

		

				 }
	
	ngOnInit() {
		   
    }



	getStockDetails() {
		this._stockService.getListWithID(this.transactionNumber)
			.subscribe( details => { 
				this.stock = details;
				console.log(this.stock);

				this.addItems(details[0].itemsToBeDelivered.map(r => this.addItems(r)));


			});
	}

	

	addItems(data: any){
		if(data.id != null)
		{
				this._productService.getRecordDetailsWithId(data.id)
				.subscribe(detail => {
					this.productDetail = detail;
					let item = this.productDetail.find(x => x.id == data.itemId);
					this.stockDetail.push(item);

					this.productDetail = item;
					console.log(this.productDetail);
				})

		const control = <FormArray>this.SRDetailsFrom.controls['itemsToBeDelivered'];
		let newItemRow = this.fb.group({

			"id":[data["id"],Validators.required],
			"stDeliveryId":[data["stDeliveryId"],Validators.required],
			"stInventoryDetailedId" : [data["stInventoryDetailedId"],Validators.required],
			"itemId":[data["itemId"],Validators.required],
			"deliveredQuantity": [data["deliveredQuantity"],Validators.required],
			"remarks": [data["remarks"],Validators.required]







		})
		this.stockDetail.push(data);
		control.push(newItemRow);
		}


	}
		   get itemsToBeDelivered(): FormArray{
        return this.SRDetailsFrom.get('itemsToBeDelivered') as FormArray;
    }


  
       


}