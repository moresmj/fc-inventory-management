import { Component, Input, Output, EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

@Component({
	selector : 'app-bo-advanced-search',
	templateUrl : './bo-advanced-search.html'
})

export class BranchOrdersAdvancedSearchComponent {
	
	// Flag for list if filtered.
	isFiltered : boolean = true;

	savedSearchForm : FormGroup;
	savedstoreCompanyRelation : any = [];

	searchForm : FormGroup;

	storeCompanyRelation : any = [];
	storeCompanyRelationList : any = [];
	storeList : any = [];
	requestStatusList : any = [];

	requestStatus : any  = [];
	savedRequestStatus : any = [];



	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();

	
	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	load() {

    	this._commonViewService.getCommonList("stores")
                     			.subscribe(ddl => { this.storeList = ddl; }); 

     	this._commonViewService.getCommonList("storecompanyrelation",true)
								 .subscribe(ddl => { this.storeCompanyRelationList = ddl; }); 

		this._commonViewService.getCommonList("requeststatus",true)
								 .subscribe(ddl => { this.requestStatusList = ddl;
													 this.requestStatusList = this.requestStatusList.filter(x => x["value"] != 3) 
												   });
					
		// For Default Loading
		this.requestStatus = ["2"];
        this.savedRequestStatus = ["2"];
								 


	}


	createForm() {
		this.searchForm = this.fb.group({
			orderedBy : new FormControl(''),
			transactionNo : new FormControl(''),
			poDateFrom : new FormControl(''),
			poDateTo : new FormControl(''),
		});
	}

	chkEventRequest(event)
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

	chkEvent(event)
	{
		let value = event.currentTarget.value;

		if (event.target.checked) {
			if (this.storeCompanyRelation.indexOf(value) == -1) {
				this.storeCompanyRelation.push(value);
			}	
		}
		else {

			let index = this.storeCompanyRelation.indexOf(value)

			if (index != -1) {
				this.storeCompanyRelation.splice(index, 1);
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
		this.savedstoreCompanyRelation = Object.assign([] , this.storeCompanyRelation ); 

		let formData = Object.assign([] , this.searchForm.value );  ;
		formData["storeCompanyRelation"] = this.storeCompanyRelation;
		formData["requestStatus"] = this.requestStatus;

		$("#advanceSearch").modal("hide");
		this.updatePage.emit(formData);
	}

	onCancel() {
		if(!this.isFiltered) {
			this.createForm();
			this.storeCompanyRelation = [];

			// Uncheck checkboxes
			$("input[name='chkStoreCompanyRelation']").prop("checked",false);
			$("input[name='chkRequestStatus']").prop("checked",false);
		}
		else
		{
			if (this.savedSearchForm != undefined) {
				this.resetCriteriaFromSearch(this.savedSearchForm.value);
			}
			$("input[name='chkstoreCompanyRelation']").prop("checked",false);
			for(let i = 0; i < this.savedstoreCompanyRelation.length; i++) {
				switch(this.savedstoreCompanyRelation[i]) { 
				    case "1": { 
				      	$("#chkStoreCompanyRelation0").prop("checked",true);
						break; 
   					}
				    case "2": { 
				      	$("#chkStoreCompanyRelation1").prop("checked",true); 
						break; 
   					} 
				}
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
		}
	}	

	onClear() {
		this.createForm();
		this.isFiltered = false;
		this.storeCompanyRelation = [];
		this.requestStatus = [];

		// Uncheck checkboxes
		$("input[name='chkStoreCompanyRelation']").prop("checked",false);
		$("input[name='chkRequestStatus']").prop("checked",false);

		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}

}