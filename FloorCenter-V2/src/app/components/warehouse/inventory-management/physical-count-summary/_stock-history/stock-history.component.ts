import { Component, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { BaseService } from '@services/base.service';
import { CommonViewService } from '@services/common/common-view.service';

import { StockHistory } from '@models/stock-history/stock-history.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { ActivatedRoute } from '@angular/router';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';


@Component({
	selector : 'app-stock-history',
	templateUrl : './stock-history.html'
})

export class PhysicalCountSummaryStockHistoryComponent {

	search : string;
	module : string = "w-stock-history";

	allRecords : any = [];
	historyList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	onHand : number = 0;

	itemId : any;
    itemDetails : any;

	public now: Date = new Date();
	pipe = new DatePipe('en-US');


	constructor(
		private route: ActivatedRoute,
		private baseService : BaseService,
		private _commonViewService : CommonViewService) 
	{
		// Param is the item id
		this.itemId = route.snapshot.params['id'];
		this.baseService.param = this.itemId;
		this._commonViewService.getCommonList("items").subscribe(dll => { 
			this.itemDetails = dll.filter(p => p["id"] == this.itemId)[0];
			console.log(this.itemDetails);
		} );
	}

	@ViewChild(PagerComponent)
	private pager : PagerComponent;


	getHistoryList(pagerModel : any) {

	    this.allRecords = pagerModel["allRecords"];
	    this.historyList =  pagerModel["pageRecord"]; 
	    this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
	    this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

	    this.onHand = this.getSum(this.allRecords);
	    this.checkStock(this.historyList);

	    console.log(this.historyList);
	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecord() {

		if (this.search == "" && this.historyList.length == 0) {
			this.pager["loadPageRecord"](1);
		}
		else{
			this.pager["filterPageRecord"](this.search);
		}
	     
	}

	toModel(detail : any): StockHistory {
		let model = new StockHistory({
		    transactionNo : detail.transactionNo,
        transaction : detail.transaction,
		    poNumber : detail.poNumber,
		    drNumber : detail.drNumber,
		    origin : detail.origin,
		    destination : detail.destination,  
		    transactionDate : (detail.transactionDate != null) ? new Date(detail.transactionDate).toLocaleString() : '',    
		    stock : detail.stock       
	  	});

		Object.getOwnPropertyNames(model).forEach(key => {
		  model[key] = (model[key]) ? model[key] : '';
		});

		return model;
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
		headers: ['Transaction No.','Transaction','PO No.', 'DR No.', 'Origin', 'Destination', 'Transaction Date', 'Stock']
	    
	};

	let title = this.now;
	let record = this.allRecords.map(r => this.toModel(r));

	new Angular2Csv(record, title.toISOString(), options);
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


}
