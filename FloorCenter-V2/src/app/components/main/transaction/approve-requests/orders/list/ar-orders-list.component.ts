import { Component, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { ApproveRequests } from '@models/approve-requests/approve-requests.model';

import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import 'rxjs/add/operator/map';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { RequestService } from '@services/request.service';
import { formControlBinding } from '@angular/forms/src/directives/reactive_directives/form_control_directive';


@Component({
	selector: 'app-ar-orders-list',
	templateUrl: './ar-orders-list.html'
})

export class ApproveRequestOrdersListComponent {
	
	search : string;
	module : string = "approveRequestsOrders";

 	allRecords : ApproveRequests[] = [];
	approveRequestList : ApproveRequests[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	isDefaultLoad : boolean = true;
	isDefaultFiltereredLoading : boolean = true;
	defaultFilter : any;

	showSaveBtn : boolean;
	displayType : string;
	orderDetails : ApproveRequests;
	updateForm : FormGroup;

	filterParam : any = [];
	transactionTypeList : any  = [];
	paymentModeList : any = [];

	selectedId : number;
	public now: Date = new Date();

	constructor(private fb: FormBuilder,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService) 
	{
		this.defaultFilter = { "requestStatus" : [2] };
   		this.filterParam = this.defaultFilter;
		this.load();
	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;


	private load(): void {
    
    	this._commonViewService.getCommonList("transactiontypes",true)
                     			.subscribe(ddl => { 
                     				this.transactionTypeList = ddl; 
                     				this.transactionTypeList = this.transactionTypeList.filter(x => x["value"] != 4);
								 });

		
	

 	}

	getApproveRequest(pagerModel : any) {
        // if (this.isDefaultLoad) {
        //     let defaultFilter = { "requestStatus" : [2] };
        //     this.filterRecordWithParam(defaultFilter);

        //     this.isDefaultLoad = false;
        // }
        // else
        // {
			this.allRecords = pagerModel["allRecords"];
	        this.approveRequestList =  pagerModel["pageRecord"]; 
	        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
	        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        // }

        console.log(this.approveRequestList);
	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecord() {

		let params = null;

		if (this.filterParam == undefined) {
			params = { "TransactionType" : this.search };
		}
		else
		{
			params = this.filterParam;
			params["TransactionType"] = this.search;
		}



		this.pager["filterPageWithParams"](1,params);  

	}

	filterRecordWithParam(event : any) {

		if (event == "loadPageRecord") {
	    	this.pager["filterPageWithParams"](1,this.filterParam);  
		}
		else
		{
			this.filterParam = event;
			event["TransactionType"] = this.search;

		    this.pager["filterPageWithParams"](1,event);  
		}
  
	}
 

	onBtnUpdateClick(data : any) {

		if (data.orderType == 1) {
			this.displayType = "store";
		}
		else if (data.orderType == 2 && (data.deliveryType == 1 || data.deliveryType == 2)) {
			this.displayType = "client";
		}
		else
		{
			this.displayType = "store";
		}

		this.orderDetails = data;
		console.log(data.id);

		this.updateForm = this.fb.group({
			id : new FormControl(data.id),
			transactionNo : new FormControl(data.transactionNo),
			orderedItems: this.fb.array([]) // here
		});


		console.log(data["requestStatus"]);
		this.showSaveBtn = (data["requestStatus"]  == 2) ? true : false;
		console.log(this.showSaveBtn);

		for(let i = 0; i < data["orderedItems"].length; i++)
		{
			const control = <FormArray>this.updateForm.controls['orderedItems'];
			let item = data["orderedItems"][i];

			let newItem = this.fb.group({
                id : new FormControl(item["id"]),
                stOrderId : new FormControl(data["id"]),
                itemId : new FormControl(item["itemId"]),
                approvedQuantity : new FormControl(item["requestedQuantity"],[Validators.required,CustomValidator.approvedQuantity]),
				approvedRemarks : new FormControl(''),
				isTonalityAny: new FormControl(item.isTonalityAny)
	        })
	        // add new formgroup
	        control.push(newItem);
		}
	}


	toModel(detail : any): ApproveRequests {
	    let model = new ApproveRequests({
	      Id : detail.id,
	      TransactionNo : detail.transactionNo,
	      OrderTypeStr : (detail.orderTypeStr != null) ? detail.orderTypeStr : '',
	      RequestStatusStr : detail.requestStatusStr,
	      StoreName : detail.orderedBy,
		  WarehouseName : detail.orderedTo,
		  payMentModeStr : (detail.payMentModeStr != null) ? detail.payMentModeStr : '',
	      PONumber : detail.poNumber,
	      PODate : (detail.orderedDate != null) ? new Date(detail.orderedDate).toLocaleString().slice(0,10).replace(",","") : ''
	    });

	    return model;
	}

	downloadUserList(){

		this._requestService.action = "transactions/approverequests";


		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.','Order Type', "Status" , "Ordered By", "Ordered To", "Payment Mode", "PO No.", "Ordered Date"]				
		};


		var param = this.filterParam;
		param["showAll"] = true;
		param["TransactionType"] = this.search == undefined ? '' : this.search;

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
						
		
	}


	

}