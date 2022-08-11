import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';


@Component({
	selector: 'app-return-advanced-search',
	templateUrl: 'return-advanced-search.html'
})



export class ReturnAdvancedSearchComponent{

	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;
  savedRequestStatus : any = [];

	
	@Output() aSearch: EventEmitter<any> = new EventEmitter<any>();

	searchForm : FormGroup;
	requestStatus : any  = [];
  returnTypeList : any = [];
  requestStatusList : any  = [];


	constructor(private fb : FormBuilder,
    private _commonViewService: CommonViewService)
	{
		this.createForm();
    this.loadDropDown();
	}


  private loadDropDown(): void{
    this._commonViewService.getCommonList("returnType",true)
                            .subscribe(ddl => {
                              this.returnTypeList = ddl;
                            });

    this._commonViewService.getCommonList("orderStatus",true)
                            .subscribe(ddl => {
                              this.requestStatusList = ddl;
                            });

  }



	createForm() {
		this.searchForm = this.fb.group({
			ReturnType : new FormControl(''),
			TransactionNo : new FormControl(''),
      ReturnFormNumber : new FormControl(''),
      RequestDateFrom : new FormControl(''),
      RequestDateTo : new FormControl(''),
      orderStatus : new FormControl('')
		});
	}


	onSearch()
	{
	let formValue = Object.assign([] , this.searchForm.value); ;

		  this.savedSearchForm = Object.assign([] , this.searchForm);
       this.savedRequestStatus = Object.assign([] , this.requestStatus ); 

          this.isFiltered = true;


		formValue["orderStatus"] = this.requestStatus;

		
		console.log(formValue);

		this.aSearch.emit(formValue);
		$("#advanceSearch").modal("hide");
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



        onCancel() {
    if(!this.isFiltered) {
      this.createForm();
      this.requestStatus = [];

      // Uncheck checkboxes
      $("input[name='chkRequestStatus']").prop("checked",false);
    }
    else
    {


      this.resetCriteriaFromSearch(this.savedSearchForm.value);
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

       resetCriteriaFromSearch(formData : any) {
        for(let key in formData) {
          console.log(key + "  " + formData[key]);
                      console.log(this.searchForm.value);
          this.searchForm.get(key).setValue(formData[key]);
    }
}

  onClear() {
    this.createForm();
    this.isFiltered = false;
    this.requestStatus = [];

    // Uncheck checkboxes
    $("input[name='chkRequestStatus']").prop("checked",false);

    // Refresh the records filtered.
    let formData = this.searchForm.value;
    formData["orderStatus"] = this.requestStatus;

    this.aSearch.emit(formData);
  }




	
}
