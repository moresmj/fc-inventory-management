import { Component, Input, Output, EventEmitter,ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';
import { AuditTrail } from '@models/audit-trail/audit-trail.model';
import { CustomValidator } from '@validators/custom.validator';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { RequestService } from '@services/request.service';

declare var jquery:any;
declare var $:any;


@Component({
	selector : 'app-audit-trail-list',
	templateUrl : './audit-trail-list.html'
})

export class AuditTrailListComponent {
	
	module : string = "audit-trail";
	// Flag for list if filtered.
	isFiltered : boolean = true;
	// 
	savedSearchForm : FormGroup;

	search : string;
	allRecords : any = [];
	auditTrailList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	searchForm : FormGroup;
	userList : any = [];
	filterParam : any;
	Keyword : any = [];

	errorMessage : any;

	public now: Date = new Date();
	pipe = new DatePipe('en-US');

	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();



	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService,
				private _requestService : RequestService)
	{
		this.createForm();
		this.load();
	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;

	load() {

    	this._commonViewService.getCommonList("users")
                     			.subscribe(ddl => { this.userList = ddl; });  

	}


	createForm() {
		this.searchForm = this.fb.group({
			dateFrom : new FormControl(''),
			dateTo : new FormControl(''),
			userID : new FormControl(''),
		});
	}

	getAuditList(pagerModel : any) {

		this.allRecords = pagerModel["allRecords"];
        this.auditTrailList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

        console.log(this.auditTrailList);
	}



	resetCriteriaFromSearch(formData : any) {
		for(let key in formData) {
			this.searchForm.get(key).setValue(formData[key]);
		}		
	}


	onSubmit() {

	let formData = this.searchForm.value;
	console.log(formData);


		
	this.Keyword = formData;
	this.pager["filterPageWithParams"](1,this.Keyword);
	}

	onCancel() {
		if(!this.isFiltered) {
			this.createForm();
		}
		else
		{
			if (this.savedSearchForm != undefined) {
				this.resetCriteriaFromSearch(this.savedSearchForm.value);
			}
		}
	}	

	onClear() {
		this.searchForm.reset();
		this.isFiltered = false;

		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}


		toModel(detail : any): AuditTrail {
	    let model = new AuditTrail({
	      	id : detail.id,
	 		date : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString() : '',
	 		userName : detail.userName,
	 		action : detail.action,
	 		transaction : detail.transaction,
	 		detail : detail.detail
	    });

	    return model;
	}

	downloadList(){
		
		this._requestService.action = "users/usertrail";

		this.Keyword["showAll"] = true;
    	var param = this.Keyword;
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Date','Username','Action','Transaction','Detail']				
		};

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