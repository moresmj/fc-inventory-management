import { Component, ViewChild } from '@angular/core';

import { Returns } from '@models/returns/returns.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { BaseComponent } from '@components/common/base.component';
import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';

@Component({
	selector: 'app-return-list',
	templateUrl: 'return-list.html'
})



export class ReturnListComponent extends BaseComponent{

	module : string = "returns";

	errorMessage: any;
 	statusMessage : any;

 	details : any;

	allRecords : any = [];
	returnList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	public now: Date = new Date();

	constructor(private _requestService : RequestService,
				private _commonViewService : CommonViewService)
	{
		super();
	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;


	getReturns(pagerModel : any) {

		this.allRecords = pagerModel["allRecords"];
        this.returnList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        this.errorMessage = pagerModel["errorMessage"];

        console.log(this.returnList);
	}

	reloadRecord(event : string) {
		if(this.pager[event]){
			this.pager[event]();
		}
	}
	
	filterRecordWithParam(event : any) {

		this.Keyword = event;
        this.Keyword["currentPage"] = 1;
		this.pager["filterPageWithParams"](1,event);    
	}


	onBtnDetailsClick(data : any) {
		this.details = data;
	}

	toModel(detail : any): Returns {

		Object.getOwnPropertyNames(detail).forEach(key => {
		  detail[key] = (detail[key]) ? detail[key] : '';
		});

	    let model = new Returns({
	      id : detail.id,
	      transactionNo : detail.transactionNo,
	      returnFormNumber : detail.returnFormNumber,
	      returnTypeStr : detail.returnTypeStr,
	      requestDate : (detail.requestDate != '') ? new Date(detail.requestDate).toLocaleString().slice(0,10).replace(",","") : '',
	      returnedTo : detail.returnedTo,
	      requestStatusStr : detail.requestStatusStr,
	      orderStatusStr : detail.orderStatusStr,
	    });

	    return model;
	}

	onBtnAdvanceSearchClick()
	{
		this.errorMessage = null;
	}

	downloadList(){
		
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.','RF No.', "Return Type", "Request Date" , "Return To", "Status", "Return Status"]				
		};

		
		this._requestService.action = "returns";
		var param = this.Keyword;
			param["showAll"] = true;

		this._requestService.getListWithParams(param)
							.subscribe(list =>{
							
								let record = list["list"].map(r => this.toModel(r));
								let title = this.now;
								new Angular2Csv(record, title.toISOString(), options);
							},
							
            error =>{
			  var errorMessage = this._commonViewService.getErrors(error);
			  console.log(errorMessage);
            });
		// let title = this.now;
		// let record = this.allRecords.map(r => this.toModel(r));

		// new Angular2Csv(record, title.toISOString(), options);
	}


}