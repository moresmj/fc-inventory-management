import { Component, ViewChild  } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreInventories } from '@models/store-inventories/store-inventories.model';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

import { OrderTransferDetailsComponent } from '@s_stock_req/order-transfer-list/details/order-transfer-details.component';
import { PagerComponent } from '@pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import 'rxjs/add/operator/map';

@Component({
	selector : 'app-order-transfer-list',
	templateUrl : './order-transfer-list.html'
})

export class OrderTransferListComponent {

  	public now : Date = new Date();	
	module : string = 'storeInventories';

	selectedInventory : StoreInventories;
	transactionTypesList : Dropdown[];

	allRecords : StoreInventories[] = [];
	storeInventoryList : StoreInventories[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	searchForm : FormGroup;

	@ViewChild(OrderTransferDetailsComponent)
	private details: OrderTransferDetailsComponent;

	@ViewChild(PagerComponent)
	private pager: PagerComponent;


	constructor(private _storeInventoriesService: StoreInventoriesService, private _commonViewService: CommonViewService,  private fb: FormBuilder)
	{
		this.load();
		this.createSearchForm();
	}

	load(): void {

		this._commonViewService.getCommonList("transactionTypes",true)
            					.subscribe(ddl => {this.transactionTypesList = ddl; });  

	}

	createSearchForm() {
		this.searchForm = this.fb.group({
	      TransactionType : new FormControl(''),
	      PONumber : new FormControl(''),
	      PODateFrom : new FormControl(''),
	      PODateTo: new FormControl(''),
	      RequestStatus : new FormControl('')
	    });
	}


	reloadRecord(event : string) {
	    if(this.pager[event]) {
		      this.pager[event]();    
		}
	}

	filterRecord() {
	    this.pager["filterPageRecord"](this.searchForm.value);	       
	}


	getList(pagerModel : any): void {
	    this.allRecords = pagerModel["allRecords"];
	    this.storeInventoryList =  pagerModel["pageRecord"]; 
	    this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
	    this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
	}

	viewDetails(data : any) {
		this.details["displayDetails"](data);
	}


	downloadStoreList() {
      var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction Number','Transaction Type', 'Status', 'PO. Number', 'PO/Request Date', 'Requested By', 'Deliver From', 'DR Number', 'DR Date']   
       };
       let title = this.now;
       let record = this.allRecords.map(r => this.toModel(r));

       new Angular2Csv(record, title.toISOString(), options);
  	}

	toModel(detail : any): StoreInventories {
	    let model = new StoreInventories({
	      Id : detail.id,
	      TransactionTypeStr : detail.transactionTypeStr,
	      RequestStatusStr : '', // Not yet sepecified.
	      PONumber : detail.PONumber,
	      PODate : detail.PODate,
	      StoreName : detail.store.name,
	      WarehouseName : detail.warehouse.name,
	      DRNumber : '', // Not yet sepecified,
	      DRDate : '' // Not yet sepecified
	    });

	    return model;
	  }


}