import { Component, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { BranchOrder } from '@models/branch-order/branch-order.model';

import { CustomValidator } from '@validators/custom.validator';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';

@Component({
	selector : 'app-ar-transfers-list',
	templateUrl : './ar-transfers-list.html'
})

export class ApproveRequestTransfersListComponent {

	module : string = "approveRequestsTransfers";

 	allRecords : any = [];
	branchOrdersList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	details : any;
	updateForm : FormGroup;

	isDefaultLoad : boolean = true;

	filterParam : any = [];
	public now: Date = new Date();
	defaultFilter : any;

	constructor(private fb: FormBuilder,
	private _commonViewService : CommonViewService,
	private _requestService : RequestService) 
	{
	this.defaultFilter = { "requestStatus" : [2] };
   	this.filterParam = this.defaultFilter;

	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;

	getBranchOrders(pagerModel : any) {

        // if (this.isDefaultLoad) {
        //     // let defaultFilter = { "requestStatus" : [2] };
        //     // this.filterRecordWithParam(defaultFilter);

        //     this.isDefaultLoad = false;
        // }
        // else
        // {
			this.allRecords = pagerModel["allRecords"];
			this.branchOrdersList =  pagerModel["pageRecord"]; 
			this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
			this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
		// }
		console.log(this.allRecords);
      
	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecordWithParam(event : any) {

		if (event == "loadPageRecord") {
			this.filterParam = { requestStatus : [2] };
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
		this.updateForm = this.fb.group({
			id : new FormControl(data["id"]),
			transactionNo : new FormControl(data["transactionNo"]),
			transferredItems: this.fb.array([]) // here
		});

		// Interbranch : append orNumber
		// InterCompany : append siNumber

		if (data["requestStatus"] == 2) {

			for(let i = 0; i < data["orderedItems"].length; i++)
			{
				const control = <FormArray>this.updateForm.controls['transferredItems'];
				let item = data["orderedItems"][i];

				let newItem = this.fb.group({
	                id : new FormControl(item["id"]),
	                stOrderId : new FormControl(item["stOrderId"]),
	                itemId : new FormControl(item["itemId"]),
	                approvedQuantity : new FormControl(item["requestedQuantity"],[Validators.required,CustomValidator.approvedQuantity]),
					approvedRemarks : new FormControl(''),
					isTonalityAny : new FormControl(item.isTonalityAny)
		        })
		        // add new formgroup
		        control.push(newItem);
			}
			
		}

	}


	toModel(detail : any): BranchOrder {
	    let model = new BranchOrder({
	      id : detail.id,
	      transactionNo : detail.transactionNo,
	      poNumber : detail.poNumber,
	      orderedBy : detail.orderedBy,
		  orderedTo : detail.orderedTo,
		  paymentModeStr : detail.paymentModeStr,
	      companyRelation : detail.storeCompanyRelationStr,
	      clientName : detail.clientName,
	      deliveryTypeStr : detail.deliveryTypeStr,
	      poDate : (detail.poDate != null) ? new Date(detail.poDate).toLocaleString().slice(0,10).replace(",","") : '',
	      requestStatusStr : detail.requestStatusStr
	    });

	    return model;
	}

	downloadList(){
		this._requestService.action = "transactions/approverequests/transfers";

    	var param = this.filterParam;
		param["showAll"] = true;

		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.','PO. No.','Ordered By', 'Ordered To', 'PaymentMode', 'Company Relation', 'Client Name', 'Delivery Mode', 'Ordered Date', 'Status']				
		};
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