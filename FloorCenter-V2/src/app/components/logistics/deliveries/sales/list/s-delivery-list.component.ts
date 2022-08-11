import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { SalesOrder } from '@models/sales-order/sales-order.model';
import { CommonViewService } from '@services/common/common-view.service';

import { PagerComponent } from '@common/pager/pager.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { RequestService } from '@services/request.service';


@Component({
	selector: "app-s-delivery-list",
	templateUrl : "./s-delivery-list.html"
})

export class SalesDeliveryListComponent {
	
  module : string = "deliveries-sales";

  allRecords : any = [];
  deliveryList : any = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  showSaveBtn : boolean;
  updateForm : FormGroup;

  delivery : any;

  

  isDefaultFiltereredLoading : boolean = true;
  defaultFilter : any;

  filterParam : any = [];
  public now: Date = new Date();

 constructor(private fb: FormBuilder,
  private _commonViewService : CommonViewService,
  private _requestService : RequestService) 
 {
    this.defaultFilter = { DeliveryStatus : [2] };
    this.filterParam = this.defaultFilter;
 }

  @ViewChild(PagerNewComponent)
  private pager : PagerNewComponent;


  getDeliveries(pagerModel : any) {
        this.allRecords = pagerModel["allRecords"];
        this.deliveryList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 


        console.log(this.deliveryList);
  }

 

  reloadRecord(event : string){
    if(this.pager[event]){
      this.pager[event]();
    }
  }

  filterRecordWithParam(event : any) {

     if (event == "loadPageRecord") {
      this.filterParam = { DeliveryStatus : [2] };
      this.pager["filterPageWithParams"](1,this.filterParam); 
     }
     else
     {
          this.filterParam = event;
          this.pager["filterPageWithParams"](1,event);    
     }
  
  }

  onBtnUpdateClick(data : any) {

   
    this.showSaveBtn = true;


 /*   if(data["approvedDeliveryDate"] == null)
    {
      let approvedDeliveryDate = data["requestedDeliveryDate"].substring(0,10);

      this.updateForm = this.fb.group({
        approvedDeliveryDate : new FormControl(approvedDeliveryDate,Validators.required),
        driverName : new FormControl(data["driverName"],Validators.required),
        plateNumber : new FormControl(data["plateNumber"],Validators.required),
      });
       
    }*/

    if(data["deliveryStatus"] != 1)
    {
      let approvedDeliveryDate = "";
      if(data["deliveryStatus"] == 2)
      {
        approvedDeliveryDate = data["requestedDeliveryDate"].substring(0,10);
      }
      else{
        approvedDeliveryDate = data["approvedDeliveryDate"].substring(0,10);

      }
       
      this.updateForm = this.fb.group({
        id : new FormControl(data["id"]),
        approvedDeliveryDate : new FormControl(approvedDeliveryDate,Validators.required),
        driverName : new FormControl(data["driverName"],[Validators.required, Validators.maxLength(100)]),
        transactionNo : new FormControl(data["transactionNo"]),
        plateNumber: new FormControl(data["plateNumber"], [Validators.required, Validators.maxLength(10)]),
        Delivered: new FormControl(data["Delivered"])
      });

    }
    else {

      this.showSaveBtn = false;
      data["approvedDeliveryDate"] = data["approvedDeliveryDate"].substring(0,10);
      
        if (data["delivered"] == 3) {
            this.updateForm = this.fb.group({
              id : new FormControl(data["id"]),
              IsDelivered : new FormControl(false),
            });
        }
      
    }

    this.delivery = data; 



  }

  toModel(detail : any): SalesOrder {
      let model = new SalesOrder({
        TransactionNo : detail.transactionNo,
        DRNumber : detail.drNumber,
        DeliverFrom : detail.orderedBy,
        DeliverTo : detail.clientName,
        DeliveryStatusStr : (detail.isCustomDelivery) ?  detail.deliveredStr : detail.deliveryStatusStr, 
        RequestedDeliveryDate : (detail.requestedDeliveryDate != null) ? new Date(detail.requestedDeliveryDate).toLocaleString().slice(0,10).replace(",","") : '',
        ApprovedDeliveryDate : (detail.approvedDeliveryDate != null) ? new Date(detail.approvedDeliveryDate).toLocaleString().slice(0,10).replace(",","") : '',
        DeliveryQty : detail.deliveryQty
      });

      Object.getOwnPropertyNames(model).forEach(key => {
        model[key] = (model[key]) ? model[key] : '';
      });

      return model;
  }


  downloadList(){

    this._requestService.action = "deliveries/sales";
    var param = this.filterParam;
		param["showAll"] = true;
    
    var options = {
      fieldSeparator: ',',
      quoteStrings: '"',
      decimalseparator: '.',
      showLabels: true,
      headers: ['Transaction No.', 'DR No.', 'Deliver From', 'Deliver To', 'Delivery Status', 'Requested Delivery Date', 'Approved Delivery Date', 'Delivery Qty']

        
    };
    this._requestService.getListWithParams(param)
							.subscribe(list =>{
							
								let record = list["list"].map(r => this.toModel(r));
								let title = this.now;
								new Angular2Csv(record, title.toISOString(), options);
							},
							
            error =>{
			  var errorMessage = this._commonViewService.getErrors(error);
			  console.log(errorMessage);
            });
  }



}
