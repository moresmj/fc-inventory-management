import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { SalesOrder } from '@models/sales-order/sales-order.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { BaseComponent } from '@components/common/base.component';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { RequestService } from '@services/request.service';


@Component({
	selector: 'app-so-list',
	templateUrl: './so-list.html'
})

export class SalesOrderListComponent extends BaseComponent {
	
	module : string = "sales-order";

 	allRecords : SalesOrder[] = [];
	salesOrderList : SalesOrder[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	public now: Date = new Date();

	constructor(private fb: FormBuilder,
				private _commonViewService : CommonViewService,
				private _requestService : RequestService) 
	{
		super();
	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;


	getSalesOrder(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
        this.salesOrderList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

        console.log(this.salesOrderList);

	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}


	filterRecordWithParam(event : any) {

		this.Keyword = event;
        this.Keyword["currentPage"] = 1;
		this.pager["filterPageWithParams"](1,event);    

	}


	toModel(detail : any): SalesOrder {
	    let model = new SalesOrder({
	      Id : detail.id,
	      TransactionNo : detail.transactionNo,
	      SINumber : detail.siNumber,
	      SalesDate : (detail.salesDate != null) ? new Date(detail.salesDate).toLocaleString().slice(0,10).replace(",","") : '',
	      ClientName : detail.clientName,
	      DeliveryTypeStr : detail.deliveryTypeStr,
	      OrderStatusStr : detail.orderStatusStr,
	    });

	    return model;
	}

	downloadList(){
		
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.','SI No.', "Sales Date", "Customer Name" , "Delivery Type", "Order Status"]				
		};

		this._requestService.action = "transactions/salesorder";
		this.Keyword["showAll"] = true;
		var param = this.Keyword;
	
		this._requestService.getListWithParams(param)
				.subscribe(list =>{
				  console.log(list);
				  this.Keyword["showAll"] = false;
					let record = list["list"].map(r => this.toModel(r));
					let title = this.now;
					new Angular2Csv(record, title.toISOString(), options);
					
				},
				error =>{
				  this.errorMessage = this._commonViewService.getErrors(error);
				});
	}


	

}