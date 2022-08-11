import { Component, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';

import { BaseService } from '@services/base.service';
import { ApiBaseService } from '@services/api-base.service';

import { StockHistory } from '@models/stock-history/stock-history.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { ActivatedRoute } from '@angular/router';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';


@Component({
	selector : 'app-w-stock-history',
	templateUrl : './w-stock-history.html'
})

export class WarehouseStockHistoryComponent {

	search : string;
	module : string = "w-stock-history";

	allRecords : any = [];
	historyList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	onHand : number = 0;
	reserved : number = 0;
	branchBroken : number  = 0;
	notReleased : number =0;
	whBroken : number  = 0;
	visibleStock : number = 0;
	brokenAfterPhyCount : number =0;

	itemId : any;
    itemDetails : any;

	public now: Date = new Date();
  	pipe = new DatePipe('en-US');


	constructor(
		private route: ActivatedRoute,
		private baseService : BaseService,
		private _apiBaseService : ApiBaseService)
	{
		this.itemId = route.snapshot.params['id'];
		this.baseService.param = this.itemId;
		this._apiBaseService.action = "items"

		let itemFilter = { 'id' : this.itemId };
		this._apiBaseService.getListWithParam(itemFilter).subscribe(data => { 
			this.itemDetails = data[0];
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
	    console.log(this.historyList, this.onHand);

	    this.checkStock(this.historyList);

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
	        transactionNo: detail.transactionNo,
	        transaction: detail.transaction,
			poNumber : detail.poNumber,
			drNumber : detail.drNumber,
			origin : detail.origin,
			destination : detail.destination,
			condition : detail.broken == true ? "Broken" : "",
			releaseDate : (detail.releaseDate != null) ? new Date(detail.releaseDate).toLocaleString().slice(0,10).replace(",","") : '',
			stock : detail.stock,
			remarks : detail.remarks       
	  	});

		Object.getOwnPropertyNames(model).forEach(key => {
		  model[key] = (model[key]) ? model[key] : '';
		});

		return model;
	}


	getSum(records : any): number {
		let total = 0;
		let date = 0;
		this.brokenAfterPhyCount =0;

		this.reserved = 0;
		this.notReleased =0;
		this.branchBroken = 0;
		this.whBroken = 0;
		this.visibleStock = 0;

        for (let i = 0; i < records.length; i++) {
			total += parseInt(records[i]["stock"]);
			total += records[i]["reserved"]=== null ? 0 : parseInt(records[i]["reserved"]);

			// date = records[i]["transaction"] === "Physical Count" && records[i]["broken"] === false && date == 0? 
			// records[i]["transactionDate"] : continue;


			//Get total resered
			this.reserved += records[i]["reserved"]=== null ? 0 : parseInt(records[i]["reserved"]);
			this.visibleStock += parseInt(records[i]["stock"]);

			//Get total branch 
			this.branchBroken += records[i]["broken"] === true && records[i]["stock"] > 0 ? parseInt(records[i]["stock"]) : 0;
			this.whBroken += records[i]["broken"] === true && records[i]["stock"] < 0 ? parseInt(records[i]["stock"]) : 0;

			this.notReleased += records[i]["deliveryStatus"] == 3 && !records[i]["releaseDate"] ? parseInt(records[i]["stock"]) : 0;
			if( records[i]["transaction"] === "Physical Count" && records[i]["broken"] === false && date == 0){
				date =records[i]["transactionDate"]
				
			}
			else{
				//console.log( date ==0  && records[i]["broken"] === true && records[i]["stock"] > 0,  records[i]["transactionDate"] +" > "+ date ,records[i]["stock"])
				this.brokenAfterPhyCount += date ==0  && records[i]["broken"] === true && records[i]["stock"] > 0? records[i]["stock"] : 0;
				
				continue;
			}

		}	
		console.log(this.branchBroken, date, this.brokenAfterPhyCount)
		return total;
	}

	downloadList(){

	var options = {
		fieldSeparator: ',',
		quoteStrings: '"',
		decimalseparator: '.',
		showLabels: true,
        headers: ['Transaction No.', 'Transaction','PO No.', 'DR No.', 'Origin', 'Destination','Condition','Release Date','Stock','Remarks']
	    
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
