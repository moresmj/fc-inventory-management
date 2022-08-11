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
	selector: 'app-ar-returns-list',
	templateUrl: './ar-returns-list.html'
})

export class ApproveRequestReturnsListComponent {
	
	module : string = "approveRequestsReturns";

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
		this.showSaveBtn = (data["requestStatus"]  == 1 || data["requestStatus"]  == 3 ) ? false : true;
		this.updateForm = this.fb.group({
			id : new FormControl(data["id"])
		});

	}


	toModel(detail : any): Returns {
	    let model = new Returns({
	      id : detail.id,
	      transactionNo : detail.transactionNo,
	      returnFormNumber : detail.returnFormNumber,
	      returnTypeStr : detail.returnTypeStr,
	      returnedBy : detail.returnedBy,
	      returnedTo : detail.returnedTo,
	      requestStatusStr : detail.requestStatusStr,
	      requestDate : (detail.requestDate != null) ? new Date(detail.requestDate).toLocaleString().slice(0,10).replace(",","") : ''
	    });

	    return model;
	}

	downloadUserList(){

		this._requestService.action = "transactions/approverequests/returns";

		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.','RF No','Return Type', "Returned By", "Returned To" , "Status", "Request Date"]				
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