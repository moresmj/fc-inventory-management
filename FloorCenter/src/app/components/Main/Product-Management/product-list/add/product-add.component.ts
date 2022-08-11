import { Component, OnInit,EventEmitter,Output,ViewChild} from '@angular/core';
import { FormControl,FormGroup,FormBuilder, Validators } from '@angular/forms';

import{ Product } from'@models/product/product.model';
import { ProductService } from '@services/product/product.service';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';


@Component({
    selector: 'app-product-add',
    templateUrl: './product-add.html'
})

export class ProductAddComponent implements OnInit{
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
	@ViewChild("fileInput") fileInput;
	product: Product;
	itemForm: FormGroup;
	errorMessage: any;
	statusMessage: string;

	sizeList: Dropdown[] = [];
  
    public buttonClicked: boolean = false

    public onButtonClick(){
        this.buttonClicked = !this.buttonClicked;
    }
    constructor(private _productService: ProductService, private fb: FormBuilder,
    	private _commonViewService: CommonViewService){

    	this.loadDropDown();


    }

    private loadDropDown(): void{
    	this._commonViewService.getCommonList("sizes")
    						   .subscribe(ddl =>{this.sizeList = ddl; });
    }

    ngOnInit(){
    	
    	this.itemForm = this.fb.group({

    		 'Code': new FormControl('',Validators.required),
      		 'Name': new FormControl('', Validators.required),
      		 'SerialNumber': new FormControl('', Validators.required),
      		 'SizeId': new FormControl('',Validators.required),
     		 'Tonality': new FormControl('',Validators.required),
      		 'Remarks': new FormControl(''),
      		 'Description': new FormControl('',),
      		 'Image' : new FormControl('')
     	
    	});
  
        
    }


 	   createForm(){
    	this.itemForm = this.fb.group({

    		 Code:['',Validators.required],
      		 Name: ['', Validators.required],
      		 SerialNumber: ['', Validators.required],
      		 SizeId: ['',Validators.required],
     		 Tonality: ['',Validators.required],
      		 Remarks: [''],
      		 Description: ['',]
     		
    	});
    }

    reset(){
      this.createForm();
      this.errorMessage = null;
      this.statusMessage = null;
    }


    addRecord(){

    	let item = this.itemForm.value;
    	let image = this.fileInput.nativeElement;

    	item["Image"] = image.files;



    	this._productService.addRecord(item)
    	.subscribe(successCode =>{
    		this.createForm();
    		this.statusMessage ="Item successfully Registered";
    		this.errorMessage = null;
    		this.updatePage.emit("loadPageRecord");
    	},
    	error =>{
    		this.errorMessage = this._commonViewService.getErrors(error);
        this.statusMessage = null;
    	});
    }

  
}