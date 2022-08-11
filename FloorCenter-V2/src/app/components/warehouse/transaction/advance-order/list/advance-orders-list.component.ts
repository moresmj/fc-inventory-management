import { Component, ViewChild, OnInit } from '@angular/core';
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

import { PageModuleService } from '@services/common/pageModule.service';
import { RequestStatusEnum } from '@models/enums/request-status-enum.model';
import { AssignmentEnum } from '@models/enums/assignment-enum.model';
import { OrderStatusEnum } from '@models/enums/order-status-enum.model';
import { AdvanceOrderDetailsComponent } from '../details/advance-orders-details.component';



@Component({
	selector: 'app-warehouse-advance-orders-list',
	templateUrl: './advance-orders-list.html'
})

export class AdvanceOrderListComponent implements OnInit {
	
	module : string = "warehouse-advance-order";

 	allRecords : any = [];
	approveRequestList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	assignment : any;

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

	ngOnInit() {

		this.assignment = parseInt(this.pageModuleService.assignment);
		this.defaultFilter = this.assignment == AssignmentEnum.Store ? { "requestStatus" : [2] } : { "orderStatus" : [2] };
		this.filterParam = this.defaultFilter;
	
	}

	constructor(private fb: FormBuilder,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService,
		private pageModuleService: PageModuleService,) 
	{
		
		

   		
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

		this.showSaveBtn = (data["orderStatus"]  == OrderStatusEnum.Completed || data["orderStatus"]  == RequestStatusEnum.Cancelled ) ? false : true;

		if(this.assignment == AssignmentEnum.Store)
		{
			this.showSaveBtn = false;
		}
		
		this.updateForm = this.fb.group({
			id : new FormControl(data["id"]),
			stOrderId : new FormControl(data["stOrderId"]),
			requestStatus : new FormControl(1),
			forUpdate : new FormControl(1),
			orderStatus : new FormControl(data["orderStatus"]),
			changeStatusReason : new FormControl(data["changeStatusReason"],Validators.required),

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
		  secondAddress : (detail.address2 == null) ? detail.address2 = '' : detail.address2,
		  thirdAddress : (detail.address3 == null) ? detail.address3 = '' : detail.address3,
		  clientName : detail.clientName,
		  ContactNo : ( detail.contactNumber == null) ? detail.contactNumber = '' : detail.contactNumber,
		  requestDate : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString().slice(0,10).replace(",","") : '',
		 // aoDate : detail.aoDate,
		  deliveryStatus : detail.deliveryStatusStr,
		  poNumber : (detail.poNumber == null) ? detail.poNumber = '' : detail.poNumber,
		  remarks : detail.remarks,  
		  returnFormNumber : detail.siNumber,
		  salesAgent : detail.salesAgent,
		  orderedBy : detail.orderedBy,
		  returnTypeStr : detail.orderedTo,
		  requestStatus : detail.requestStatusStr,
		  approveDate : (detail.approveDate != null) ? new Date(detail.approveDate).toLocaleString().slice(0,10).replace(",","") : '',
		  orderStatus : (detail.orderStatusStr == null || detail.orderStatusStr == undefined ) ? detail.orderStatusStr = '' : detail.orderStatusStr,
		  paymentMode : (detail.paymentModeStr == undefined || detail.paymentModeStr == null) ? detail.paymentModeStr = '' : detail.paymentModeStr,
		  
		//   code : detail.advanceOrderDetails[0].itemCode,
		//   size : detail.advanceOrderDetails[0].sizeName,
		//   tonality : (detail.advanceOrderDetails[0].tonality == null || detail.advanceOrderDetails[0].tonality == undefined ) ? detail.advanceOrderDetails[0].tonality = '' : detail.advanceOrderDetails[0].tonality ,
		//   approvedQuantity : (detail.advanceOrderDetails[0].approvedQuantity == null || detail.advanceOrderDetails[0].approvedQuantity == undefined ) ? detail.advanceOrderDetails[0].approvedQuantity = '' : detail.advanceOrderDetails[0].approvedQuantity,
		//   allocatedQuantiy : detail.advanceOrderDetails[0].quantity,
	     // requestStatusStr : (detail.orderStatusStr != undefined) ? (detail.requestStatusStr + '- ' + detail.orderStatusStr) : detail.requestStatusStr  ,
	     // requestDate : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString().slice(0,10).replace(",","") : ''
	    });

	    return model;
	}


	onAdvanceSearchClick()
	{
		this.assignment;
	}

	

	downloadUserList(){

		this._requestService.action = "transactions/orders/warehouse/advanceorder";

		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			//headers: ['ID','AO No.','Address1', 'Address2','Address3',"Client Name", 'Contact Number','AO Date', 'Delivery Status', 'PONumber', 'Remarks', 'SI Number','Sales Agent', 'Store', "Warehouse", "Request Status", "Approved Date", "Order Status", "Payment Mode","Code","Size","Tonality","Approved Quantity","Allocated Quantity"]
			headers: ['ID','AO No.','Address1', 'Address2','Address3',"Client Name", 'Contact Number','AO Date', 'Delivery Status', 'PONumber', 'Remarks', 'SI Number','Sales Agent', 'Store', "Warehouse", "Request Status", "Approved Date", "Order Status", "Payment Mode"]				
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