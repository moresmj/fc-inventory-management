import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { OrderService } from '@services/order/order.service';
import { StoreOrder } from '@models/store-order/store-order.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
	selector : 'app-receive-items-list',
	templateUrl : './receive-items-list.html'
})

export class ReceiveItemsListComponent {
	
	module : string = "order";

	allRecords : StoreOrder[] = [];
	receiveList : StoreOrder[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	public now: Date = new Date();

	constructor(private fb: FormBuilder, private _orderService : OrderService) 
	{
		this._orderService.action = "orders/receiveitems";
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
			TransactionTypeStr : detail.transactionTypeStr,
			DRNumber : !detail.isTransfer ? ((detail.orderType != 2 && detail.deliveryType != 3) ? 
					(detail.deliveryType == 3 && (detail.orderType == 3 && detail.orderToStoreId == detail.storeId) 
					? detail.orNumber 
					: detail.deliveries[0].drNumber) 
			: "") 
			: detail.isInterBranch ? detail.orNumber : detail.deliveries[0].drNumber
			,
			WHDRNumber : detail.transactionType == 6 ? "" : detail.whdrNumber,
			DeliveryDate : (detail.deliveries[0].deliveryDate) ? new Date(detail.deliveries[0].deliveryDate).toLocaleString().slice(0,10).replace(",","") : '',
			StoreName : detail.store.name,
			ApprovedDeliveryDate : (detail.deliveries[0].approvedDeliveryDate) ? new Date(detail.deliveries[0].approvedDeliveryDate).toLocaleString().slice(0,10).replace(",","") : ''
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
			headers: ['Transation No.','Transaction', 'TOR / DR No.','WHDR No.' ,'Ordered Date', 'Ordered By', 'Delivery Date']
		};
		let title = this.now;
		let record = this.allRecords.map(r => this.toModel(r));

		new Angular2Csv(record, title.toISOString(), options);
	}


}