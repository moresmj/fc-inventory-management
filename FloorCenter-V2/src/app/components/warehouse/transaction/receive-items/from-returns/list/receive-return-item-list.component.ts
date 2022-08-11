import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { ReceiveItems } from '@models/receive-items/receive-items.model';


import { PagerComponent } from '@common/pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
declare var $jquery: any;
declare var $: any;

@Component({
    selector: 'app-receive-return-item-list',
    templateUrl: './receive-return-item-list.html'
})

export class ReceiveReturnItemListComponent implements AfterViewInit {
     public now: Date = new Date();


 searchSuccess: any;
 errorMessage: any;

  allRecords : ReceiveItems[] = [];
  receivedItemsList:   ReceiveItems[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;
  selectedInventoryid : number;
  selectedInventory : any;
  updateForm : FormGroup;
  searchForm : FormGroup;


  module : string = "receive-return-items";

    constructor(private _receiveItemsService : ReceiveItemsService,
        private fb: FormBuilder)
    {
        this.createSearchForm(); 
    }

  @ViewChild(PagerComponent)
  private pager: PagerComponent;

    loadAllReceiveItems(pagerModel : any){

      if(pagerModel["errorMessage"] != null)
      {
        this.allRecords = null;
        this.receivedItemsList = null;
        this.totalRecordMessage = "0 Records Found";
        this.pageRecordMessage = null;
        this.errorMessage = pagerModel["errorMessage"];

        console.log(this.allRecords);


      }
      else{
         this.allRecords = pagerModel["allRecords"];
        this.receivedItemsList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        this.errorMessage = pagerModel["errorMessage"];
        console.log(this.allRecords);

      }

       
    }

    onBtnViewDetailClick(data : any): void{
        this.selectedInventoryid = data.id;
        this.selectedInventory = data;

       

        this.updateForm = this.fb.group({


            id : new FormControl(data.id,Validators.required),

            deliveredItems : this.fb.array([])



        });
    }


    createSearchForm(){
        this.searchForm = this.fb.group({


            returnFormNumber : new FormControl(),
            drNumber : new FormControl(),
            transactionNumber : new FormControl()




        });
    }

    clearMessage(){
      this.errorMessage = null;
    }

    onSearch(){
    let formData = this.searchForm.value;


      if (formData == "" && formData.length == 0) {
        this.pager["loadPageRecord"](1);
      }

    this.pager["filterPageWithParams"](formData);
      console.log(formData);
    }

    onSearch2(data: any){

      
    this.pager["filterPageRecord2"](data);
    this.searchSuccess = true;
  
    }

    filterRecordWithParam(event : any) {
      this.pager["filterPageWithParams"](event);
      this.searchSuccess = true;    
  }


      reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  } 



 downloadRecords(){
   var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction No.', 'RF No.', 'DR No.','Requested Date','Returned By','Delivery Date']   
      };
      let title = this.now;
      let record = this.allRecords.map(r => this.toModel(r));

      new Angular2Csv(record, title.toISOString(), options);
  }



  toModel(detail : any): ReceiveItems{
    let model = new ReceiveItems({
 
    transactionNo: detail.transactionNo,
    returnFormNumber  : detail.returnFormNumber,
    drNumber: detail.drNumber,
    requestedDate: detail.requestedDate,
    returnedBy: detail.returnedBy,
    deliveryDate : new Date(detail.deliveryDate).toLocaleDateString(),
  
  
   
  

    });
    return model;
  }



    async ngAfterViewInit() {
        $(document).ready(function(){
            $("#rDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("rDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        })
    }
}