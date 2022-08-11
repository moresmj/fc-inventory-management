import { Component, AfterViewInit,OnInit,Input,Output,ViewChild,ViewChildren,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { ReceiveItems } from '@models/receive-items/receive-items.model';

import { ReceiveItemListComponent } from '@warehouse/transaction/receive-items/list/receive-item-list.component';
import { Dropdown } from '@models/common/dropdown.model';
import { AssignmentEnum } from '@models/enums/assignment-enum.model';

declare var jquery:any;
declare var $:any;


@Component({

	selector:'app-receive-item-advance-search',
	templateUrl: 'receive-item-advance-search.html'

})


export class ReceiveItemAdvanceSearchComponent
{

    // Flag for list if filtered.
  isFiltered : boolean = false;
  // 
  savedSearchForm : FormGroup;
  savedRequestStatus : any = [];

	@Output() aSearch: EventEmitter<any> = new EventEmitter<any>();
	@Input()recordCount:string;
	@Input()successSearch:any;
	advanceSearchForm : FormGroup;


  userId : any;
	useridList : any = [];

	@ViewChild(ReceiveItemListComponent)
  	private rItemList: ReceiveItemListComponent;


	constructor(private fb: FormBuilder,
		private _commonViewService: CommonViewService)
	{
    	this.loadDropDown();
		this.createSearchForm();


	}


	 private loadDropDown(): void{
    this._commonViewService.getCommonList("users")
                            .subscribe(ddl => {
                              this.useridList = ddl;
                              this.useridList = this.useridList.filter(x => x.assignment == AssignmentEnum.Warehouse);
                              console.log(this.useridList) 
                          });
 }


	 createSearchForm(){
        this.advanceSearchForm = this.fb.group({


            poNumber : new FormControl(""),
            drNumber : new FormControl(""),
            poDateFrom : new FormControl(""),
            poDateTo : new FormControl(""),
            drDateFrom : new FormControl(""),
            drDateTo : new FormControl(""),
            itemName: new FormControl(""),
            UserId : new FormControl("")

            
		  		// myParams.set('poNumber', param.poNumber);
		  		// myParams.set('drNumber', param.drNumber);
		  		// myParams.set('poDateFrom', param.poDateFrom);
		  		// myParams.set('poDateTo',param.poDateTo);
		  		// myParams.set('drDateFrom', param.drDateFrom);
		  		// myParams.set('drDateTo',param.drDateTo);	
		  		// myParams.set('itemName',param.itemName);
		  		// myParams.set('UserId', param.receiveBy);	  	





        });
    }


       onSearch(){
          
          this.successSearch = false;

          this.savedSearchForm = Object.assign([] , this.advanceSearchForm);
          this.savedRequestStatus = Object.assign([] , this.userId);

          this.isFiltered = true;

          let formData = Object.assign([] , this.advanceSearchForm.value); ;
          formData["requestStatus"] = this.userId;


         	this.aSearch.emit(formData);
         	$("#advanceSearch").modal("hide");



     
      }


       onCancel() {
                    if(!this.isFiltered) {
                      this.createSearchForm();
                      this.userId = [];
                    }
                    else
                    {
                      console.log(this.advanceSearchForm.value);
                      console.log(this.savedSearchForm.value);

                      this.resetCriteriaFromSearch(this.savedSearchForm.value);


                    }


      } 


      resetCriteriaFromSearch(formData : any) {
        for(let key in formData) {
          console.log(key + "  " + formData[key]);
          this.advanceSearchForm.get(key).setValue(formData[key]);
    }   
  }

 
  onClear() {
    this.createSearchForm();
    this.isFiltered = false;


    // Refresh the records filtered.
    let formData = this.advanceSearchForm.value;
    this.aSearch.emit(formData);
  }


	
}