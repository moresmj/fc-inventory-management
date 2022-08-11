import { Component, ViewChild } from '@angular/core';

import { ApiBaseService } from '@services/api-base.service';

import { StockHistory } from '@models/stock-history/stock-history.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Router, ActivatedRoute, Params } from '@angular/router';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';


@Component({
	selector : 'app-s-stock-history',
	templateUrl : './s-stock-history.html'
})

export class StoreStockHistoryComponent {

	search : string;
	module : string = "m-inv-store-stock-history";

	allRecords : any = [];
	historyList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	onHand : number = 0;

    itemDetails : any;

    isDefaultLoading : boolean = true;
    isDefaultFiltereredLoading : boolean = true;
    defaultFilter : any;

	public now: Date = new Date();

	@ViewChild(PagerComponent)
	private pager : PagerComponent;

	constructor(
		private route: ActivatedRoute,
		private _apiBaseService : ApiBaseService) 
	{
	    this.route.queryParams.subscribe(
     		data => 
     		 	{ 
     		 		this.defaultFilter = { id : data["id"], storeId : data["storeId"] };
					this._apiBaseService.action = "items"

					
					let itemFilter = { 'id' : data['id']};
					this._apiBaseService.getListWithParam(itemFilter).subscribe(data => {  
						this.itemDetails = data[0];
						console.log(this.itemDetails);
					} );
     		 	}
     		);

	}


	getHistoryList(pagerModel : any) {

	    this.allRecords = pagerModel["allRecords"];
	    this.historyList =  pagerModel["pageRecord"]; 
	    this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
	    this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

	    this.onHand = this.getSum(this.allRecords);

	    this.checkStock(this.historyList);

	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecord() {

		if (this.search == "" && this.historyList.length == 0) {
			this.pager["filterPageWithParams"](this.defaultFilter);
		}
		else{

			this.pager["filterPageWithQueryParams"](this.defaultFilter,this.search);
		}     
	}


	toModel(detail : any): StockHistory {
		let model = new StockHistory({
		    transactionNo : detail.transactionNo,
		    poNumber : (detail.poNumber) ? detail.poNumber : '' ,
		    drNumber : (detail.drNumber) ? detail.drNumber : '' ,
		    siNumber : (detail.siNumber) ? detail.siNumber : '' ,
		    origin : (detail.origin) ? detail.origin :  '',
		    destination : (detail.destination) ? detail.destination : '', 
		    transactionDate : (detail.transactionDate != null) ? new Date(detail.transactionDate).toLocaleString() : '',      
		    stock : detail.stock       
	  	});

		return model;
	}

	checkStock(data: any)
	{
		if(data.length > 0)
		{
			data.forEach((item, index) => {

				if(item.stock > 0)
				{
					item["isPositive"] = true;

				}
				else{
					item["isPositive"] = false;
					item["class"] = 'stock-row';
				}
		});
		}

	}


	getSum(records : any): number {

		let total = 0;
		for (let i = 0; i < records.length; i++ ) {
			total += parseInt(records[i]["stock"]);
		}	

		return total;
	}

	downloadList(){

	var options = {
		fieldSeparator: ',',
		quoteStrings: '"',
		decimalseparator: '.',
		showLabels: true,
		headers: ['Transaction No.','PO No.', 'TOR / DR No.', 'SI No.', 'Origin', 'Destination', 'Transaction Date', 'Stock']
	    
	};

	let title = this.now;
	let record = this.allRecords.map(r => this.toModel(r));

	new Angular2Csv(record, title.toISOString(), options);
	}


}