import { Component, Input, Output, EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { Dropdown } from '@models/common/dropdown.model';

declare var jquery:any;
declare var $:any;


@Component({
	selector: 'app-so-advanced-search',
	templateUrl: './so-advanced-search.html'
})

export class SalesOrderAdvancedSearchComponent {
	
	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;
	saveddeliveryStatus : any = [];
	savedorderStatus : any = [];

	searchForm : FormGroup;
	deliveryStatusList : Dropdown[] = [];

	orderStatusList : any = [];
	orderStatus : any = [];

	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();


	deliveryStatus : any  = [];

	constructor(private fb : FormBuilder, private _commonViewService : CommonViewService)
	{
		this.createForm();
		this.load();
	}

	load() {

    	this._commonViewService.getCommonList("deliverystatus",true)
								 .subscribe(ddl => { this.deliveryStatusList = ddl; });  
								 

		this._commonViewService.getCommonList("orderStatus",true)
                     			.subscribe(ddl => { this.orderStatusList = ddl; });  

	}


	createForm() {
		this.searchForm = this.fb.group({
			siNumber : new FormControl(''),
			salesDateFrom : new FormControl(''),
			salesDateTo : new FormControl(''),
			clientName : new FormControl(''),
		});
	}

	resetCriteriaFromSearch(formData : any) {
		for(let key in formData) {
			this.searchForm.get(key).setValue(formData[key]);
		}		
	}


	onSubmit() {

		this.isFiltered = true;
		this.savedSearchForm = Object.assign([] , this.searchForm );
		this.savedorderStatus = Object.assign([] , this.orderStatus ); 

		let formData = Object.assign([] , this.searchForm.value );  ;
		formData["deliveryStatus"] = this.deliveryStatus;
		formData["orderStatus"] = this.orderStatus;

		$("#advanceSearch").modal("hide");
		this.updatePage.emit(formData);
	}

	onCancel() {
		if(!this.isFiltered) {
			this.createForm();
			this.deliveryStatus = [];

			// Uncheck checkboxes
			$("input[name='chkdeliveryStatus']").prop("checked",false);
		}
		else
		{
			console.log(this.searchForm.value);
			console.log(this.savedSearchForm.value);

			this.resetCriteriaFromSearch(this.savedSearchForm.value);
			$("input[name='chkdeliveryStatus']").prop("checked",false);
			for(let i = 0; i < this.saveddeliveryStatus.length; i++) {
				switch(this.saveddeliveryStatus[i]) { 
				    case "1": { 
				      	$("#chkdeliveryStatus0").prop("checked",true);
						break; 
   					}
				    case "2": { 
				      	$("#chkdeliveryStatus1").prop("checked",true); 
						break; 
   					} 
				    case "3": { 
				      	$("#chkdeliveryStatus2").prop("checked",true);
						break; 
   					} 
				}
			}

			$("input[name='chkorderStatus']").prop("checked",false);
			for(let i = 0; i < this.savedorderStatus.length; i++) {
				switch(this.savedorderStatus[i]) { 
				    case "1": { 
				      	$("#chkorderStatus0").prop("checked",true);
						break; 
   					}
				    case "2": { 
				      	$("#chkorderStatus1").prop("checked",true); 
						break; 
   					} 
				    case "3": { 
				      	$("#chkorderStatus2").prop("checked",true);
						break; 
   					} 
				}
			}
		}
	}	

	onClear() {
		this.createForm();
		this.isFiltered = false;

		// Uncheck checkboxes
		$("input[name='chkdeliveryStatus']").prop("checked",false);
		$("input[name='chkorderStatus']").prop("checked",false);
		this.orderStatus = [];
		// Refresh the records filtered.
		let formData = this.searchForm.value;
		this.updatePage.emit(formData);
	}


	chkEvent(event)
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


}