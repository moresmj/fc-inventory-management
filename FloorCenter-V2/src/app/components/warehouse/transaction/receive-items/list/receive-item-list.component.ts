import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { ReceiveItems } from '@models/receive-items/receive-items.model';


import { PagerComponent } from '@common/pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { BaseComponent } from '@components/common/base.component';
import { RequestService } from '@services/request.service';
declare var $jquery: any;
declare var $: any;

@Component({
    selector: 'app-receive-item-list',
    templateUrl: './receive-item-list.html'
})

export class ReceiveItemListComponent extends BaseComponent implements AfterViewInit {
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


  module : string = "receive-items";

    constructor(private _receiveItemsService : ReceiveItemsService,
                private fb: FormBuilder,
                private _requestService : RequestService,
                private _commonViewService : CommonViewService,)
    {
        super();
        this.createSearchForm(); 
    }

  @ViewChild(PagerNewComponent)
  private pager: PagerNewComponent;

    loadAllReceiveItems(pagerModel : any){

      if(pagerModel["errorMessage"] != null)
      {
        this.allRecords = null;
        this.receivedItemsList = null;
        this.totalRecordMessage = "0 Records Found";
        this.pageRecordMessage = null;
        this.errorMessage = pagerModel["errorMessage"];


      }
      else{
         this.allRecords = pagerModel["allRecords"];
        this.receivedItemsList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        this.errorMessage = pagerModel["errorMessage"];

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


            poNumber : new FormControl(""),
            drNumber : new FormControl(""),
            poDateFrom : new FormControl(""),
            poDateTo : new FormControl("")




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

    this.Keyword = formData;
    this.Keyword["currentPage"] = 1;

    this.pager["filterPageWithParams"](1, formData);
      console.log(formData);
    }

    onSearch2(data: any){

      
    this.pager["filterPageRecord2"](data);
    this.searchSuccess = true;
  
    }

    filterRecordWithParam(event : any) {
      this.pager["filterPageWithParams"](1, event);
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
       headers: ['Transaction No.', 'PO No.', 'DR No.','Received Date']   
      };


      this._requestService.action = 'transactions/receiveitems'
      this.Keyword["showAll"] = true;
      var param = this.Keyword;
    
      this._requestService.getListWithParams(param)
          .subscribe(list =>{
            console.log(list);
            this.Keyword["showAll"] = false;
            let record = list["list"].map(r => this.toModel(r));
            let title = this.now;
            new Angular2Csv(record, title.toISOString(), options);
            
          },
          error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
          });
  }



  toModel(detail : any): ReceiveItems{
    let model = new ReceiveItems({
 
      transactionNo: detail.transactionNo,
      poNumber: detail.poNumber,
      drNumber: detail.drNumber,
      receivedDate: new Date(detail.receivedDate).toLocaleString().slice(0,10).replace(",",""),
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