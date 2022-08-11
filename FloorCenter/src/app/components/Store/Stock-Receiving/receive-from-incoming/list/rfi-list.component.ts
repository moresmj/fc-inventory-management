import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';


import { PagerComponent } from '@pager/pager.component';
import { StockService } from '@services/stock/stock.service';
import { Stock } from '@models/stock/stock.model';



@Component({
	selector : 'app-receive-from-incoming-list',
	templateUrl : './rfi-list.html'
})

export class ReceiveFromIncomingListComponent {


	  module : string = "stock";


	   allRecords : Stock[] = [];
  	   stockList : Stock[] = [];
       totalRecordMessage : string;
       pageRecordMessage : string;



	constructor(private _stockService: StockService, private fb: FormBuilder) 
  {

  }



    getStockList(pagerModel : any): void {
        this.allRecords = pagerModel["allRecords"];
        this.stockList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
  }
	
}