import { Component, AfterViewInit,OnInit,ViewChild } from '@angular/core';
import { FormControl,FormGroup,FormBuilder,Validators} from '@angular/forms';

import { PagerComponent } from '@pager/pager.component';
import { Product } from '@models/product/product.model';
import { ProductService } from '@services/product/product.service';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';


declare var jquery:any;
declare var $ :any;

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements AfterViewInit {


  SelectedId : number;
  allRecords : Product[] = [];
  productList: Product[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;


  productDlList : Product [] = [];
  itemForm: FormGroup;
  module : string = "product";

  search : string;

  public now: Date = new Date();



  @ViewChild(PagerComponent)
  private pager: PagerComponent;


	async ngAfterViewInit(){

		   $(document).ready(function(){
            $(".addNew").click(function(){
                $("#addItems").toggle();
            });
            $("#cancel").click(function(){
                $("#addItems").toggle();
            });
            $("#poDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
            });
            $("#poDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        });

        $(function() {

          // We can attach the `fileselect` event to all file inputs on the page
          $(document).on('change', ':file', function() {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, label]);
          });

          // We can watch for our custom `fileselect` event like this
          $(document).ready( function() {
              $(':file').on('fileselect', function(event, numFiles, label) {

                  var input = $(this).parents('.input-group').find(':text'),
                      log = numFiles > 1 ? numFiles + ' files selected' : label;

                  if( input.length ) {
                      input.val(log);
                  } else {
                      if( log ) alert(log);
                  }

              });
          });
          
        });

	}


  constructor(private productService: ProductService, private fb: FormBuilder)
  {

  }


  getProductList(pagerModel : any): void{
    this.allRecords = pagerModel["allRecords"];
    this.productList =  pagerModel["pageRecord"]; 
    this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
    this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
  }

  reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  }


  filterRecord() {

    if (this.search == "" && this.productList.length == 0) {
      this.pager["loadPageRecord"](1);
    }
    else{
      this.pager["filterPageRecord"](this.search);
    }
       
  }



  public onSelect(product: Product): void{


    this.SelectedId = product.id;
    this.itemForm = this.fb.group({

         Code: new FormControl(product.code,Validators.required),
         Name:new FormControl(product.name, Validators.required),
         SerialNumber: new FormControl(product.serialNumber, Validators.required),
         SizeId: new FormControl(product.sizeId,Validators.required),
         Tonality: new FormControl(product.tonality,Validators.required),
         Remarks: new FormControl(product.remarks),
         Description: new FormControl(product.description)
        
      });

  } 


  private mapProducts(r: any): Product[] {
    return r.map(pl=> this.toProduct(pl));
  }


  private toProduct(r:any):Product {

    let product = new Product({
     
    serialNumber: r.serialNumber,
    code: r.code,
    name : r.name,
    description: r.description,
    sizeId: r.sizeId,
    tonality: r.tonality,
    remarks: r.remarks,
    dateadded: new Date(r.dateCreated).toLocaleString(),
    size: r.size
   
    
   




    });

    return product;

  }

downLoadItemList(){
        var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Serial Number','Item Code','Item Name ','Description', 'Specification', 'Tonality', 'Remarks','Date Added']   
  

      };

    let title = this.now;
   this.productDlList = this.allRecords.map(r => this.toProduct(r));


    new Angular2Csv(this.productDlList, title.toISOString(), options);
}


  
}
