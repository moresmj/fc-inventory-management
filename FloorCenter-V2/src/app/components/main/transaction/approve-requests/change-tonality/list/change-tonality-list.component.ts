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
	selector: 'app-change-tonality-list',
	templateUrl: './change-tonality-list.html'
})

export class ApproveRequestChangeTonalityListComponent {
	
	module : string = "approveModifyTonality";

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
			console.log(this.allRecords);
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
		console.log(data);
		this.showSaveBtn = (data["requestStatus"]  == 1 || data["requestStatus"]  == 3 ) ? false : true;
		this.updateForm = this.fb.group({
			id : new FormControl(data["id"]),
			stOrderId : new FormControl(data["stOrderId"]),
			requestStatus : new FormControl(1),
			modifyItemTonalityDetails: this.fb.array([]) 
		});


		for(let i = 0; i < data["modifyItemTonalityDetails"].length; i++)
		{
			const control = <FormArray>this.updateForm.controls['modifyItemTonalityDetails'];
			let item = data["modifyItemTonalityDetails"][i];

			let newItem = this.fb.group({
                id : new FormControl(item["id"]),
                itemId : new FormControl(item["itemId"]),
				oldItemId : new FormControl(item["oldItemId"]),
				WHModifyItemTonalityId : new FormControl(data["id"]),
				StClientDeliveryId : new FormControl(item["stClientDeliveryId"]),
				StShowroomDeliveryId : new FormControl(item["stShowroomDeliveryId"]),
	        })
	        // add new formgroup
	        control.push(newItem);
		}

	}


	toModel(detail : any): Returns {
	    let model = new Returns({
	      id : detail.id,
	      transactionNo : detail.transactionTobeModified,
	      returnFormNumber : detail.poNumber,
	      returnTypeStr : detail.warehouse.name,
	      requestStatusStr : detail.requestStatusStr,
	      requestDate : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString().slice(0,10).replace(",","") : ''
	    });

	    return model;
	}

	downloadUserList(){

		this._requestService.action = "transactions/approverequests/modifytonality";

		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Transaction No.','PO No','Date Approved', "Warehouse",  "Status", "Request Date"]				
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