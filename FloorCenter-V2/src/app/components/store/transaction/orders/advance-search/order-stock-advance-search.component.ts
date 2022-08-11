import { Component, AfterViewInit,OnInit,Input,Output,ViewChild,ViewChildren,OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';

import { ActivatedRoute, Router } from '@angular/router';
import { OrderStockListComponent } from '@store/transaction/orders/list/order-stock-list.component';
import { Dropdown } from '@models/common/dropdown.model';

declare var jquery:any;
declare var $:any;


@Component({

  selector:'app-order-stock-advance-search',
  templateUrl: 'order-stock-advance-search.html'

})


export class OrderStockAdvanceSearchComponent
  {

  // Flag for list if filtered.
  isFiltered : boolean = false;
  // 
  savedSearchForm : FormGroup;
  savedRequestStatus : any = [];
  savedOrderStatus : any = [];

  @Output() aSearch: EventEmitter<any> = new EventEmitter<any>();
  @Input()recordCount:string;
  @Input()successSearch:any;
  advanceSearchForm : FormGroup;

  transactionTypeList : any = [];
  
  requestStatusList : any = [];
  orderStatusList : any = [];

  deliveryType : any = [];
  requestStatus : any  = [];
  orderStatus : any  = [2];


  filterRS : any;

  constructor(private fb: FormBuilder,
    private route: ActivatedRoute,
    private _commonViewService: CommonViewService)
  {
      this.loadDropDown();
      this.createSearchForm();

      this.route.queryParams.subscribe(
        data =>
        {
          this.filterRS = parseInt(data.filter) +1;
          if(this.filterRS != 1 && this.filterRS != 2){
            this.filterRS = "";
          }
        });
  }


  private loadDropDown(): void{
    this._commonViewService.getCommonList("requeststatus",true)
              .subscribe(ddl => { this.requestStatusList = ddl; });  
              
    this._commonViewService.getCommonList("transactiontypes",true)
              .subscribe(ddl => {
                let cntr =0
                for(let i =0; i < ddl.length; i++){
                  if(ddl[i].name == "PO" || ddl[i].name =="Transfer"){     
                    this.transactionTypeList[cntr] = ddl[i]
                    cntr++;
                  }
                }
              }); 

    this._commonViewService.getCommonList("orderstatus",true)
              .subscribe(ddl => { this.orderStatusList = ddl; }); 
  }


   createSearchForm(){
        this.advanceSearchForm = this.fb.group({

      transactionType : new FormControl(''),
      poNumber : new FormControl(''),
      poDateFrom : new FormControl(''),
      poDateTo : new FormControl(''),
      transactionNo : new FormControl(''),
      remarks : new FormControl('')


        });
    }

  resetCriteriaFromSearch(formData : any) {
    for(let key in formData) {
      console.log(key + "  " + formData[key]);
      this.advanceSearchForm.get(key).setValue(formData[key]);
    }   
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
  
    chkEventOrder(event)
    {
        let value = event.currentTarget.value;
        value = parseInt(value);

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


    onSearch(){

        this.savedSearchForm = Object.assign([] , this.advanceSearchForm );
        this.savedRequestStatus = Object.assign([] , this.requestStatus ); 
        this.savedOrderStatus = Object.assign([] , this.orderStatus ); 

        this.isFiltered = true;

    let formData = Object.assign([] , this.advanceSearchForm.value ); ;

    this.successSearch = false;

    // formData["deliveryType"] = this.deliveryType;
    formData["requestStatus"] = this.requestStatus;
    formData["orderStatus"] = this.orderStatus;
    console.log(formData);


    this.aSearch.emit(formData);

    $("#advanceSearch").modal("hide");
     
      }


  onCancel() {
  if(!this.isFiltered) {
    this.createSearchForm();
    this.requestStatus = [];
    this.orderStatus = [];

    // Uncheck checkboxes
    $("input[name='chkRequestStatus']").prop("checked",false);
    $("input[name='chkOrderStatus']").prop("checked",false);
  }
    else
    {
    console.log(this.advanceSearchForm.value);
    console.log(this.savedSearchForm.value);

    this.resetCriteriaFromSearch(this.savedSearchForm.value);
    $("input[name='chkRequestStatus']").prop("checked",false);
    $("input[name='chkOrderStatus']").prop("checked",false);
    
    // REQUEST STATUS
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

    // ORDER STATUS
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
    this.createSearchForm();
    this.isFiltered = false;

    // Uncheck checkboxes
    $("input[name='chkRequestStatus']").prop("checked",false);
    $("input[name='chkOrderStatus']").prop("checked",false);

    // Refresh the records filtered.
    let formData = this.advanceSearchForm.value;
    this.aSearch.emit(formData);
  } 





  
}