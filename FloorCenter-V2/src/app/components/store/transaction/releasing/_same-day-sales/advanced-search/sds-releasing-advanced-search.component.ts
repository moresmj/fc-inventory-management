import { Component, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';




@Component({
	selector: 'app-sds-releasing-advanced-search',
	templateUrl: 'sds-releasing-advanced-search.html'
})



export class SameDaySalesReleasingAdvancedSearchComponent{

	// Flag for list if filtered.
	isFiltered : boolean = false;
	// 
	savedSearchForm : FormGroup;
	// savedRequestStatus : any = [];

	
	@Output() aSearch: EventEmitter<any> = new EventEmitter<any>();

	searchForm : FormGroup;
	requestStatus : any  = [];

	constructor(private fb : FormBuilder)
	{
		this.createForm();
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


		formValue["requestStatus"] = this.requestStatus;

		
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


    // Refresh the records filtered.
    let formData = this.searchForm.value;
    this.aSearch.emit(formData);
  }




	
}
