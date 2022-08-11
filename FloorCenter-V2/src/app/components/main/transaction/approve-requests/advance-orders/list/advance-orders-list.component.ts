import { Component, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { Returns } from '@models/returns/returns.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { RequestService } from '@services/request.service';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';


@Component({
	selector: 'app-advance-orders-list',
	templateUrl: './advance-orders-list.html'
})

export class ApproveAdvanceOrderListComponent {
	
	module : string = "approve-advance-order";

 	allRecords : any = [];
	approveRequestList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	isDefaultLoad : boolean = true;
	isDefaultFiltereredLoading : boolean = true;
	defaultFilter : any;


	showSaveBtn : boolean;
	details : any;
	updateForm : FormGroup;

	filterParam : any = [];
	transactionTypeList : any  = [];

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
                     			.subscribe(ddl => { this.transactionTypeList = ddl; });  

 	}

	getApproveRequest(pagerModel : any) {
 
			this.allRecords = pagerModel["allRecords"];
	        this.approveRequestList =  pagerModel["pageRecord"]; 
	        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
			this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
			console.log(this.allRecords)
	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecordWithParam(event : any) {

		if (event == "loadPageRecord") {
	    	this.pager["filterPageWithParams"](1,this.filterParam);  
		}
		else
		{
			this.filterParam = event;
		    this.pager["filterPageWithParams"](1,event);  
		}
  
	}
 

	onBtnUpdateClick(data : any) {

		this.details = data;
		console.log(data);
		this.showSaveBtn = (data["requestStatus"]  == 1 || data["requestStatus"]  == 3 ) ? false : true;
		this.updateForm = this.fb.group({
			id : new FormControl(data["id"]),
			stOrderId : new FormControl(data["stOrderId"]),
			requestStatus : new FormControl(1),
			forUpdate : new FormControl(1),
			advanceOrderDetails: this.fb.array([]) 
		});


		for(let i = 0; i < data["advanceOrderDetails"].length; i++)
		{
			const control = <FormArray>this.updateForm.controls['advanceOrderDetails'];
			let item = data["advanceOrderDetails"][i];

			let newItem = this.fb.group({
				id : new FormControl(item["id"]),
				stAdvanceOrderId : new FormControl(item["stAdvanceOrderId"]),
				itemId : new FormControl(item["itemId"]),
				forUpdate : new FormControl(1),
				approvedQuantity: new FormControl(item["quantity"],[Validators.required,CustomValidator.approvedQuantity])
	        })
	        // add new formgroup
	        control.push(newItem);
		}

	}


	toModel(detail : any): Returns {
	    let model = new Returns({
			id : detail.id,
			transactionNo : detail.aoNumber,
			firstAddress : detail.address1,
			secondAddress : (detail.address2 == null ) ? detail.address2 = '' : detail.address2,
			thirdAddress : (detail.address3 == null ) ? detail.address3 = '' : detail.address3,
			clientName : detail.clientName,
			ContactNo : detail.contactNumber,
			requestDate : (detail.dateCreated != null ) ? new Date(detail.dateCreated).toLocaleString().slice(0,10).replace(",","") : '',
		   // aoDate : detail.aoDate,
			deliveryStatus : detail.deliveryStatusStr,
			poNumber : (detail.poNumber == null) ? detail.poNumber = '' : detail.poNumber ,
			remarks : (detail.remarks == null) ? detail.remarks = '' : detail.remarks ,  
			returnFormNumber : detail.siNumber,
			salesAgent : detail.salesAgent,
			orderedBy : detail.orderedBy,
			returnTypeStr : detail.orderedTo,
			requestStatus : detail.requestStatusStr,
			approveDate : (detail.approveDate != null) ? new Date(detail.approveDate).toLocaleString().slice(0,10).replace(",","") : '',
			orderStatus : (detail.orderStatusStr == null || detail.orderStatusStr == undefined ) ? detail.orderStatusStr = '' : detail.orderStatusStr,
			paymentMode : detail.paymentModeStr,
			// code : detail.advanceOrderDetails[0].itemCode,
			// size : detail.advanceOrderDetails[0].sizeName,
			// tonality : (detail.advanceOrderDetails[0].tonality == null || detail.advanceOrderDetails[0].tonality == undefined ) ? detail.advanceOrderDetails[0].tonality = '' : detail.advanceOrderDetails[0].tonality ,
			// approvedQuantity : (detail.advanceOrderDetails[0].approvedQuantity == null || detail.advanceOrderDetails[0].approvedQuantity == undefined ) ? detail.advanceOrderDetails[0].approvedQuantity = '' : detail.advanceOrderDetails[0].approvedQuantity,
			// allocatedQuantiy : detail.advanceOrderDetails[0].quantity,

		});
		console.log("DETAILLLLL",detail)

	    return model;
	}

	downloadUserList()
	{
		this._requestService.action = "transactions/approverequests/advanceorders";

		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			//headers: ['ID','AO No.','Address1', 'Address2','Address3',"Client Name", 'Contact Number','AO Date', 'Delivery Status', 'PONumber', 'Remarks', 'SI Number','Sales Agent', 'Store', "Warehouse", "Request Status", "Approved Date","Order Status", "Payment Mode","Code","Size","Tonality","Approved Quantity","Allocated Quantity"]
			headers: ['ID','AO No.','Address1', 'Address2','Address3',"Client Name", 'Contact Number','AO Date', 'Delivery Status', 'PONumber', 'Remarks', 'SI Number','Sales Agent', 'Store', "Warehouse", "Request Status", "Approved Date","Order Status", "Payment Mode"]				
		};
		
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
	}

}