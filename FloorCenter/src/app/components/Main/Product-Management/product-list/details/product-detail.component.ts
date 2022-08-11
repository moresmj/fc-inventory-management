import { Component,OnChanges,SimpleChanges, OnInit,Input,Output,EventEmitter } from '@angular/core';
import { FormGroup, FormControl,Validators,FormBuilder } from '@angular/forms';

import { ProductService } from '@services/product/product.service';
import { CommonViewService } from '@services/common/common-view.service';

import { Product } from '@models/product/product.model';
import { Dropdown } from '@models/common/dropdown.model';





@Component({
    selector: 'app-product-detail',
    templateUrl: './product-detail.html'
})

export class ProductDetailComponent implements OnInit{

	@Input() updateItemForm: FormGroup;
	@Input() itemId : number;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();


	sizeList: Dropdown[] =[];
	errorMessage : any;
	statusMessage: string;
	
	ngOnChanges(changes: SimpleChanges){
		if(changes["updateItemForm"])
		{
			this.errorMessage = null;
			this.statusMessage = null;
	
		}
	}


	constructor(
		private fb : FormBuilder,
		private _productService : ProductService,
		private _commonViewService: CommonViewService ){

		this.loadDropDown();
	}

    ngOnInit(){

        
    }

      private loadDropDown(): void{
    	this._commonViewService.getCommonList("sizes")
    						   .subscribe(ddl =>{this.sizeList = ddl; });
    }


    onSubmit(){

    	let item = this.updateItemForm.value;


    	this._productService.updateRecord(this.itemId,item)
    	.subscribe(successCode =>{		
    		this.statusMessage ="Item successfully updated";
    		this.errorMessage = null;
    		this.updatePage.emit("loadPageRecord");
    		
    	},

    	error =>{
    		this.errorMessage = this._commonViewService.getErrors(error);
    	});
    }
}