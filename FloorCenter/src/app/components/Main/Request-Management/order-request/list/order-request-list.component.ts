import { Component, AfterViewInit,ViewChild } from '@angular/core';
import { FormGroup, FormControl,Validators,FormBuilder,FormArray } from '@angular/forms';



import { PagerComponent } from '@pager/pager.component';
import { OrderRequestService } from '@services/order_request/order-request.service';
import { OrderRequest } from '@models/order_request/order-request.model';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

declare var $query: any;
declare var $: any;

@Component({
    selector: 'app-order-request-list',
    templateUrl: './order-request-list.html'
})

export class OrderRequestListComponent implements AfterViewInit {
    public now: Date = new Date();


    selectedRequestId: Number;
    selectedRequestItems: any;
    selectedRequest: any;


    allRecords : OrderRequest[] = [];
    orderRequestList : OrderRequest[] = [];
    totalRecordMessage : string;
    pageRecordMessage: string;
    updateForm : FormGroup;
    statusMessage: string;



    module : string = "order_request";




    @ViewChild(PagerComponent)
    private pager: PagerComponent;



    constructor(private _orderRequestService : OrderRequestService,
        private fb: FormBuilder) { 

    }

   


    onBtnViewDetailClick(data : any): void{
        this.selectedRequestId = data.id;
        this.selectedRequestItems = data.requestedItems;
        this.selectedRequest = data;
        this.statusMessage = null;

        this.updateForm = this.fb.group({


            id : new FormControl(data.id,Validators.required),

            requestedItems : this.fb.array([])



        })





        
    }

     reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  }

  showMessage(event : string){
    this.statusMessage = event;

  }


    getOrderRequest(pagerModel : any): void{

        this.allRecords = pagerModel["allRecords"];
        this.orderRequestList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
    }


    downloadOrderRequestList(){
   var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction No.','Transaction','Status', 'PO. No.', 'PO/Request Date', 'Store', 'DR No.', 'DR Date']   
      };
      let title = this.now;
      let record = this.allRecords.map(r => this.toModel(r));

      new Angular2Csv(record, title.toISOString(), options);
  }



  toModel(detail : any): OrderRequest{
    let model = new OrderRequest({
     id : detail.id,
     transactionTypeStr : detail.transactionTypeStr,
     requestStatusStr : detail.requestStatusStr,
     poNumber : detail.poNumber,
     poDate : detail.poDate,
     storeId : detail.storeId
 
    
   
  

    });
    return model;
  }

    async ngAfterViewInit() {
        $(document).ready(function(){
            $('#rDate').daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $('#rDate2').daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            })
        });
    }
}
