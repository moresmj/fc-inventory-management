import { Component, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { BranchOrder } from '@models/branch-order/branch-order.model';

import { CustomValidator } from '@validators/custom.validator';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerComponent } from '@common/pager/pager.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';

import { CommonViewService } from '@services/common/common-view.service';

import { RequestService } from '@services/request.service';

import { CookieService } from 'ngx-cookie-service';

import 'rxjs/add/operator/map';

@Component({
	selector : 'app-bo-list',
	templateUrl : './bo-list.html'
})

export class BranchOrdersListComponent {

	module : string = "branchOrders";

	defaultFilter : any;
	userType : any;

 	allRecords : any = [];
	branchOrdersList : BranchOrder[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	details : any;
	updateForm : FormGroup;

	isDefaultLoad : boolean = true;
	isDefaultFiltereredLoading : boolean = true;

	filterParam : any = [];
	public now: Date = new Date();

	Keyword : any = [];

	constructor(private fb: FormBuilder,
		private _requestService : RequestService,
		private _commonViewService : CommonViewService,
		private _cookieService : CookieService,) 
	{
		this.defaultFilter = { "requestStatus" : [2] };
		this.Keyword  = this.defaultFilter;
		this._requestService.action = "branchOrders";
		this.userType = this._cookieService.get('userType');

	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;

	getBranchOrders(pagerModel : any) {
			this.allRecords = pagerModel["allRecords"];
			this.branchOrdersList =  pagerModel["pageRecord"]; 
			this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
			this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
			console.log(this.Keyword);
        
	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecordWithParam(event : any) {

		if (event == "loadPageRecord") {
			this.Keyword = this.filterParam;
	    	this.pager["filterPageWithParams"](1,this.filterParam);  
		}
		else
		{
			this.Keyword = event;
		    this.pager["filterPageWithParams"](1,event);  
		}
  
	}

	onBtnUpdateClick(data : any) {


		this.updateForm = this.fb.group({
			id : new FormControl(data["id"]),
			transactionNo: new FormControl(data["transactionNo"])
		});

		// Interbranch : append orNumber
		// InterCompany : append siNumber

		if (data["isInterbranch"] && data["orNumber"] == null) {
			data["isEditable"] = true;
			this.updateForm.addControl('orNumber', new FormControl('',[Validators.required, Validators.maxLength(50)])); 
		}
		else if (!data["isInterbranch"] && data["siNumber"] == null)
		{
			data["isEditable"] = true;
			this.updateForm.addControl('siNumber', new FormControl('',[Validators.required, Validators.maxLength(50)]));
			this.updateForm.addControl('whdrNumber', new FormControl('',[Validators.required, Validators.maxLength(50)]));
		}
		else
		{
			data["isEditable"] = false;
		}


		this.details = data;
	}


	toModel(detail : any): BranchOrder {
	    let model = new BranchOrder({
	      id : detail.id,
	      transactionNo : detail.transactionNo,
	      poNumber : detail.poNumber,
	      orderedBy : detail.orderedBy + "(" + detail.storeCompanyRelationStr + ")",
	      clientName : detail.clientName,
	      deliveryTypeStr : detail.deliveryTypeStr,
	      poDate : (detail.poDate != null) ? new Date(detail.poDate).toLocaleString().slice(0,10).replace(",","") : ''
	    });

	    return model;
	}

	downloadList(){
		
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.', "PO No.", "Ordered By", "Client Name", "Delivery Mode",  "Ordered Date"]				
		};
		// let title = this.now;
		// let record = this.allRecords.map(r => this.toModel(r));
		var param = this.filterParam;
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
	

		// new Angular2Csv(record, title.toISOString(), options);
	}



}