import { Component, Input, Output, EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

declare var jquery:any;
declare var $:any;


@Component({
	selector: 'app-advance-orders-advanced-search',
	templateUrl: './advance-orders-advanced-search.html'
})

export class AdvanceOrderAdvancedSearchComponent {
	
	// Flag for list if filtered.
	isFiltered : boolean = true;
	// 
	savedSearchForm : FormGroup;
	savedRequestStatus : any = [];

	@Input() assignment : any;

	searchForm : FormGroup;
	storeList : Dropdown[] = [];

	requestStatus : any  = [];
	requestStatusList : Dropdown[] = [];
	orderStatusList : any = [];
	orderStatus : any = [];
	savedOrderStatus : any = [];
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	

	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	load() {

    	this._commonViewService.getCommonList("stores")
                     			.subscribe(ddl => { this.storeList = ddl; });  


    	this._commonViewService.getCommonList("requeststatus",true)
								 .subscribe(ddl => { this.requestStatusList = ddl; });  
								 
		this._commonViewService.getCommonList("orderstatus",true)
								 .subscribe(ddl => { this.orderStatusList = ddl;
								 });  


        // For Default loading
        this.requestStatus = ["2"];
		this.savedRequestStatus = ["2"];
		this.orderStatus = ["2"];
		this.savedOrderStatus = ["2"];

	}


	createForm() {
		this.searchForm = this.fb.group({
			// storeId : new FormControl(''),
			// transactionNo : new FormControl(''),
			// returnFormNumber : new FormControl(''),
			aoNumber : new FormControl(''),
			requestDateFrom : new FormControl(''),
			requestDateTo : new FormControl(''),
			siNumber : new FormControl(''),
			clientName : new FormControl(''),
			orderedBy : new FormControl(''),
			itemCode : new FormControl('')

		});
	}


	chkEvent(event)
	{
		let value = event.currentTarget.value;

		if (event.target.checked) {
			if (this.requestStatus.indexOf(value) == -1) {
				this.requestStatus.push(value);
			}	
		}
		else {

			let index = this.requestStatus.indexOf(value)

			if (index != -1) {
				this.requestStatus.splice(index, 1);
			}
		}
	}


	chkEvent2(event)
	{
		let value = event.currentTarget.value;

		if (event.target.checked) {
			if (this.orderStatus.indexOf(value) == -1) {
				this.orderStatus.push(value);
			}	
		}
		else {

			let index = this.orderStatus.indexOf(value)

			if (index != -1) {
				this.orderStatus.splice(index, 1);
			}
		}
	}

	resetCriteriaFromSearch(formData : any) {
		for(let key in formData) {
			this.searchForm.get(key).setValue(formData[key]);
		}		
	}


	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );
		this.savedRequestStatus = Object.assign([] , this.requestStatus ); 
		this.savedOrderStatus = Object.assign([] , this.orderStatus ); 

		let formData = Object.assign([] , this.searchForm.value );  ;
		formData["requestStatus"] = this.requestStatus;
		formData["orderStatus"] = this.orderStatus;

		$("#advanceSearch").modal("hide");
		console.log(formData);
		this.updatePage.emit(formData);
	}

	onCancel() {
		if(!this.isFiltered) {
			this.createForm();
			this.requestStatus = [];

			// Uncheck checkboxes
			$("input[name='chkRequestStatus']").prop("checked",false);
			
			$("input[name='chkOrderStatus']").prop("checked",false);
		}
		else
		{
			if (this.savedSearchForm != undefined) {
				this.resetCriteriaFromSearch(this.savedSearchForm.value);
			}
			$("input[name='chkRequestStatus']").prop("checked",false);
			for(let i = 0; i < this.savedRequestStatus.length; i++) {
				switch(this.savedRequestStatus[i]) { 
				    case "1": { 
				      	$("#chkRequestStatus0").prop("checked",true);
						break; 
   					}
				    case "2": { 
				      	$("#chkRequestStatus1").prop("checked",true); 
						break; 
   					} 
				    case "3": { 
				      	$("#chkRequestStatus2").prop("checked",true);
						break; 
   					} 
				}
			}

			$("input[name='chkOrderStatus']").prop("checked",false);
			for(let i = 0; i < this.savedOrderStatus.length; i++) {
				switch(this.savedOrderStatus[i]) { 
				    case "1": { 
				      	$("#chkOrderStatus0").prop("checked",true);
						break; 
   					}
				    case "2": { 
				      	$("#chkOrderStatus1").prop("checked",true); 
						break; 
   					} 
				    case "3": { 
				      	$("#chkOrderStatus2").prop("checked",true);
						break; 
   					} 
				}
			}
		}
	}	

	onClear() {
		this.createForm();
		this.isFiltered = false;
		this.requestStatus = [];
		this.orderStatus = [];

		// Uncheck checkboxes
		$("input[name='chkRequestStatus']").prop("checked",false);
		$("input[name='chkOrderStatus']").prop("checked",false);

		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}

}