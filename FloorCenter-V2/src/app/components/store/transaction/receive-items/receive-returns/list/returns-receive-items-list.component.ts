import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { OrderService } from '@services/order/order.service';
import { StoreOrder } from '@models/store-order/store-order.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
	selector : 'app-returns-receive-items-list',
	templateUrl : './returns-receive-items-list.html'
})

export class ReturnsReceiveItemsListComponent {
	
	module : string = "return-receive-items";

	allRecords : StoreOrder[] = [];
	receiveList : StoreOrder[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	public now: Date = new Date();

	constructor(private fb: FormBuilder) 
	{
		
	}

	@ViewChild(PagerComponent)
		private pager : PagerComponent;


	getReceiveItems(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
		this.receiveList =  pagerModel["pageRecord"]; 
		this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
		this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
		console.log(this.allRecords);
	}


	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecordWithParam(event : any) {
		this.pager["filterPageWithParams"](event);    
	}


	toModel(detail : any): StoreOrder {

		let model = new StoreOrder({
			TransactionNo : detail.transactionNo,
			TransactionTypeStr : detail.returnTypeStr,
			DRNumber : detail.returnDRNumber,
			DeliveryDate : this.checkDate(detail.requestDate),
			StoreName : detail.returnedBy,
			ApprovedDeliveryDate : this.checkDate(detail.approvedDeliveryDate)
		});

		Object.getOwnPropertyNames(model).forEach(key => {
		  model[key] = (model[key]) ? model[key] : '';
		});

		return model;
	}

	downloadList(){
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['Transation No.','Transaction', 'DR No.', 'Ordered Date', 'Returned By', 'Delivery Date']
		};
		let title = this.now;
		let record = this.allRecords.map(r => this.toModel(r));

		new Angular2Csv(record, title.toISOString(), options);
	}


	checkDate(date : string)
	{
		return (date) ? new Date(date).toLocaleString().slice(0,10).replace(",","") : '';
	}


}