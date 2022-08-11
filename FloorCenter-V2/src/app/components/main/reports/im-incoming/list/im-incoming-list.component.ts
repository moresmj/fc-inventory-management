import { Component, ViewChild } from '@angular/core';
import { FormArray, FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { Inventories } from '@models/inventories/inventories.model';
import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { CustomValidator } from '@validators/custom.validator';
import { CommonViewService } from '@services/common/common-view.service';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { BaseComponent } from '@components/common/base.component';
import { RequestService } from '@services/request.service';

@Component({
	selector : 'app-im-incoming-list',
	templateUrl : './im-incoming-list.html'
})


export class InventoryMonitoringListComponent extends BaseComponent {

	search : string;
	module : string = "incoming-inventory";

	searchForm : FormGroup;

 	allRecords : any = [];
	inventoryList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	storeList : any  = [];
	filterParam : any;

	public now: Date = new Date();

	constructor(private fb: FormBuilder,
				private _commonViewService : CommonViewService,
				private _requestService : RequestService) 
	{
		super();
		this.load();
		this.createForm();
	}

	@ViewChild(PagerNewComponent)
	private pager : PagerNewComponent;


	private load(): void {
    
    	this._commonViewService.getCommonList("stores")
                     			.subscribe(ddl => { this.storeList = ddl; });  
                     				                    			

 	}

 	createForm() {
		this.searchForm = this.fb.group({
			storeId : new FormControl('',Validators.required),
			dateFrom : new FormControl(''),
			dateTo : new FormControl('')
		});
	}

	getInventoryList(pagerModel : any) {

		this.allRecords = pagerModel["allRecords"];
        this.inventoryList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

        console.log(this.inventoryList);
	}

	reloadRecord(event : string){
		if(this.pager[event]){
			this.pager[event]();
		}
	}

	filterRecordWithParam(event : any) {

		let formData = Object.assign([] , this.searchForm.value );

		this.filterParam = event;
		let advancedSearch = this.filterParam;

		for(let key in advancedSearch)
		{
			let paramValue = advancedSearch[key];
			if (paramValue != null) {
				formData[key] = paramValue;
			}
		}

		this.Keyword = formData;
        this.Keyword["currentPage"] = 1;
		
		this.pager["filterPageWithParams"](1, formData);  
  
	}

	onSubmit() {

		let formData = Object.assign([] , this.searchForm.value );
		let advancedSearch = this.filterParam;

		for(let key in advancedSearch)
		{
			let paramValue = advancedSearch[key];
			if (paramValue != null) {
				formData[key] = paramValue;
			}
		}

		this.Keyword = formData;
		this.Keyword["currentPage"] = 1;
		
		this.pager["filterPageWithParams"](1, formData); 

	}

	toModel(detail : any): Inventories {


		Object.getOwnPropertyNames(detail).forEach(key => {
		  detail[key] = (detail[key]) ? detail[key] : '';
		});

	    let model = new Inventories({
	      Code : detail.code,
		  Description : detail.description,
		  Tonality: detail.tonality,
	      SizeName : detail.sizeName,
	      transactionDate : (detail.transactionDate != '') ? new Date(detail.transactionDate).toLocaleString().slice(0,10).replace(",","") : '',
	      drNumber : detail.drNumber,
	      poNumber : detail.poNumber,
	      adjustment : detail.adjustment,
	      origin : detail.origin,
	      fromSupplier : detail.fromSupplier,
	      fromOtherSupplier : detail.fromOtherSupplier,
	      fromInterBranch : detail.fromInterBranch,
	      fromInterCompany : detail.fromInterCompany,
	      FromSalesReturns : detail.fromSalesReturns,
	    });

	    return model;
	}

	downloadList(){
		
		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['Item Code','Description','Tonality' ,'Size','Date', "DR No.", "PO No." , "Adjustment","Origin", "From Supplier", "From Other Supplier","From Interbranch","From InterCompany", "From Sales Returns"]				
		};


		this._requestService.action =  "inventories/main/inventory-monitoring/incoming";
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
		// let title = this.now;
		// let record = this.allRecords.map(r => this.toModel(r));

		// new Angular2Csv(record, title.toISOString(), options);
	}

}
