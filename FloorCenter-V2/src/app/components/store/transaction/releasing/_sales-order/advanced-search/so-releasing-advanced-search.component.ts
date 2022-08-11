import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { CommonViewService } from '@services/common/common-view.service';




@Component({
	selector: 'app-so-releasing-advanced-search',
	templateUrl: 'so-releasing-advanced-search.html'
})



export class SalesOrderReleasingAdvancedSearchComponent{

	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;
  // savedRequestStatus : any = [];
  
  releaseStatusList : any = [];

	
	@Output() aSearch: EventEmitter<any> = new EventEmitter<any>();

	searchForm : FormGroup;
	releaseStatus : any  = [];

  constructor(private fb : FormBuilder,
    private _commonViewService : CommonViewService)
	{
    this.createForm();
    this.load();
    this.releaseStatus = [3];
	}



	createForm() {
		this.searchForm = this.fb.group({
			SINumber : new FormControl(''),
			ORNumber : new FormControl(''),
			ClientName : new FormControl(''),
		});
	}


	onSearch()
	{
	let formValue = Object.assign([] , this.searchForm.value); ;

		  this.savedSearchForm = Object.assign([] , this.searchForm);

          this.isFiltered = true;


		formValue["releaseStatus"] = this.releaseStatus;

		
		console.log(formValue);

		this.aSearch.emit(formValue);
		$("#advanceSearch").modal("hide");
	}


	chkEvent(event)
    {
        let value = event.currentTarget.value;

        if (event.target.checked) {
          if (this.releaseStatus.indexOf(value) == -1) {
            this.releaseStatus.push(value);
          } 
        }
        else {

          let index = this.releaseStatus.indexOf(value)

          if (index != -1) {
            this.releaseStatus.splice(index, 1);
          }
        }
    }



        onCancel() {
                    if(!this.isFiltered) {
                      this.createForm();
                   //   this.userId = [];
                    }
                    else
                    {
                      console.log(this.searchForm.value);
                      console.log(this.savedSearchForm.value);

                      this.resetCriteriaFromSearch(this.savedSearchForm.value);


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
    $("input[name='chkDeliveryStatus']").prop("checked",false);
    this.releaseStatus = [];


    // Refresh the records filtered.
    let formData = this.searchForm.value;
    this.aSearch.emit(formData);
  }

  private load(): void {

		this._commonViewService.getCommonList("releasestatus",true)
		                  .subscribe(ddl => { 
		               
                        this.releaseStatusList = ddl;
                        this.releaseStatusList = this.releaseStatusList.filter(p => p.value != 2)
											});

	}




	
}
