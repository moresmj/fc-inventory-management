import { Component, AfterViewInit,OnInit,Input,Output,ViewChild,ViewChildren,EventEmitter,OnChanges, SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { ReleaseItems } from '@models/release-items/release-items.model';

import { Dropdown } from '@models/common/dropdown.model';

declare var jquery:any;
declare var $:any;


@Component({

	selector:'app-change-tonality-advance-search',
	templateUrl: 'change-tonality-advance-search.html'

})


export class ChangeTonalityAdvanceSearchComponent
{

  // Flag for list if filtered.
  isFiltered : boolean = false;
  // 
  savedSearchForm : FormGroup;
  savedDeliveryType : any = [];

	@Output() aSearch: EventEmitter<any> = new EventEmitter<any>();
	@Input()recordCount:string;
	@Input()successSearch:any;
    @Input()filter : any;
	advanceSearchForm : FormGroup;
	useridList : Dropdown[] = [];
    deliveryTypeList : any;
    deliveryType : any = [];
    releaseStatusList : any = [];





	constructor(private fb: FormBuilder,
		private _commonViewService: CommonViewService)
	{
    	this.loadDropDown();
		this.createSearchForm();


	}


	 private loadDropDown(): void{

        this._commonViewService.getCommonList("deliverytypes",true).subscribe(dll => { 
                this.deliveryTypeList = dll; 
            });     
 }


	 createSearchForm(){
        this.advanceSearchForm = this.fb.group({


            poNumber : new FormControl(""),
            drNumber : new FormControl(""),
            whDrNumber : new FormControl(""),
            DeliveryDateFrom : new FormControl(""),
            DeliveryDateTo : new FormControl(""),
            deliveryType : new FormControl(""),

        });
    }


       onSearch(){


        this.successSearch = false;

        this.savedSearchForm = Object.assign([] , this.advanceSearchForm);


        this.isFiltered = true;
        var deliveryType = [];
        $('input[name="deliveryType"]:checked').each(function() {
                deliveryType.push(this.value);
              });




        let formData = Object.assign([] , this.advanceSearchForm.value); ;

        formData["deliveryType"] = deliveryType;

        this.savedDeliveryType = Object.assign([] , deliveryType ); 

       	this.aSearch.emit(formData);

       	$("#advanceSearch").modal("hide");



     
      }


        onCancel() {
                    if(!this.isFiltered) {
                      this.createSearchForm();
                   //   this.userId = [];
                    }
                    else
                    {
              

                      this.resetCriteriaFromSearch(this.savedSearchForm.value);
                      $("input[name='deliveryType']").prop("checked",false);
                      for(let i = 0; i < this.savedDeliveryType.length; i++) 
                      {
                            switch(this.savedDeliveryType[i]) { 
                                case "1": { 
                                    $("#chkDeliveryStatus0").prop("checked",true);
                                break; 
                                }
                                case "2": { 
                                    $("#chkDeliveryStatus1").prop("checked",true); 
                                break; 
                                } 
                                case "3": { 
                                    $("#chkDeliveryStatus2").prop("checked",true);
                                break; 
                                } 
                            }
                        }


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

    $("input[name='deliveryType']").prop("checked",false);  
    // Refresh the records filtered.
    let formData = this.advanceSearchForm.value;
    this.aSearch.emit(formData);
  }




  ngOnChanges(changes : SimpleChanges)
  {
    var onLoad = true;


      //check selected checkboxes base on router parameters
     for(let i = 0; i < this.filter.deliveryType.length; i++)
     {
        if (onLoad) {
          // Uncheck checkboxes
          $("input[name='deliveryType']").prop("checked",false);
          onLoad = false;
        }

        var index = parseInt(this.filter.deliveryType[i]) - 1;
        var name = "#chkDeliveryStatus"+index.toString();
        $(name).prop('checked', true);
     }


     this.isFiltered = true;
     this.savedDeliveryType = Object.assign([] , this.filter.deliveryType ); 
     this.savedSearchForm = Object.assign([] , this.advanceSearchForm);



  }


  
	
}