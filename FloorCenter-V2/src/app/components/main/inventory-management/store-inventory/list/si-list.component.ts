import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { Inventories } from '@models/inventories/inventories.model';
import { PagerComponent } from '@common/pager/pager.component';
import { Pager2Component } from '@common/pager2/pager2.component';
import { CommonViewService } from '@services/common/common-view.service';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import 'rxjs/add/operator/map';
declare var $jquery: any;
declare var $: any;

@Component({
	selector : 'app-si-list',
	templateUrl : './si-list.html'
})

export class StoreInventoryListComponent {

	currentSelectedItem : number = -1;
	selectedStore : number = 0;

	module : string = "store-inventory";

	filter : any;
	storeList : any = [];

	allRecords : any = [];
	storeInventoryList : any = [];
	totalRecordMessage : string;
	pageRecordMessage : string;

	allRecords2 : any = [];
	storeItems : any = [];
	totalRecordMessage2 : string;
	pageRecordMessage2 : string;

	details : any;
	searchForm : FormGroup;

	isDefaultLoading : boolean = true;
    isDefaultFiltereredLoading : boolean = true;
    defaultFilter : any;

	public now: Date = new Date();

	constructor(
			private fb: FormBuilder, 
			private _commonViewService : CommonViewService,   
			private route: ActivatedRoute,
    		private router: Router
		) 
	{
		this.createForm();
		this.load();

	    this.route.queryParams.subscribe(
     		data => 
     		 	{ 
     		 		this.searchForm.controls.keyword.setValue(data["keyword"]);

     		 		if (this.isDefaultLoading) {
     		 			this.defaultFilter = data;
     		 			this.isDefaultLoading = false;
     		 		}
     		 		else
     		 		{
     		 			$("#btnSearch").click();
     		 		}
     		 	}
     		);
	    console.log(this.searchForm);
	}


	@ViewChild(PagerComponent)
	private pager : PagerComponent;

	@ViewChild(Pager2Component)
	private pager2 : Pager2Component;

	load(): void {

		this._commonViewService.getCommonList("stores")
            					.subscribe(ddl => {this.storeList = ddl; }); 		
 
	}

	getStoreInventoryList(pagerModel : any) {

	    this.allRecords = pagerModel["allRecords"];
	    this.storeInventoryList =  pagerModel["pageRecord"]; 
	    this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
	    this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
	    console.log(this.storeInventoryList);
	}

	createForm() {
		this.searchForm = this.fb.group({
			storeId : new FormControl(''),
			keyword : new FormControl('')
		});
	}

	onSearch() {
		let formData = this.searchForm.value;
		this.pager["filterPageWithParams"](formData);  
	}

	panelCollapse(storeId : number, index : number)
	{
		this.selectedStore = storeId;

		// Uncollapse alldiv panels.
		$("div[name='storePanelBody']").css("display",'none');
		$('#storePanelBody' + index).slideToggle("slow");	


		this.currentSelectedItem = index;

		// Append the pager to the selected panel.
		$("#item_pager").appendTo('#divItem' + index);


		let record = this.storeInventoryList.filter(p => p.storeId == storeId);

		// Index - items of the store.
		this.pager2["currentPage"] = 1;
		this.pager2["setItemPageRecord"](record[0].items); 				
	}


	filterStoreItem(event : any) {

		let record = this.storeInventoryList.filter(p => p.storeId == this.selectedStore);
		// Index - items of the store.
		this.pager2["currentPage"] = 1;
		this.pager2["filterPageRecord"](record[0].items, event); 	
	}


	setItemPagination(pagerModel : any) {

	    this.allRecords2 = pagerModel["allRecords"];
	    this.storeItems =  pagerModel["pageRecord"]; 
	    this.totalRecordMessage2 =  pagerModel["totalRecordMessage"]; 
	    this.pageRecordMessage2 =  pagerModel["pageRecordMessage"]; 
	}

	onBtnDetailClick(data : any)
	{
		data["imagePath"] = (data["imageName"] != "") ? 'assets/images/item-images/image/' + data["imageName"] : null;
		this.details = data;
	}


	toModel(detail : any): Inventories {
		let model = new Inventories({
			ItemId : detail.itemId,
			SerialNumber : detail.serialNumber,
			Code : detail.code,
			ItemName : detail.itemName,
			SizeName : detail.sizeName,
			Tonality : detail.tonality,
			StoreName : detail.storeName,
			StoreAddress : detail.storeAddress,
			CompanyName : detail.companyName,
			OnHand : detail.onHand,
			ForRelease : detail.forRelease,
			Broken : detail.broken,
			Available : detail.available
	  });

	  return model;
	}


	downloadList(){

	var options = {
	  fieldSeparator: ',',
	  quoteStrings: '"',
	  decimalseparator: '.',
	  showLabels: true,
	  headers: ['Id', 'Serial No.','Item Code', 'Item Name', 'Size', 'Tonality', 'Store', 'Store Address', 'Company', 'On-hand', 'For Release', 'Broken', 'Available']
	    
	};
	let title = this.now;
	let record = this.allRecords2.map(r => this.toModel(r));

	new Angular2Csv(record, title.toISOString(), options);
	}



}