import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { StoreOrder } from '@models/store-order/store-order.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
	selector: "app-rd-list",
	templateUrl : "./rd-list.html"
})

export class ReturnDeliveryListComponent {
	
  module : string = "deliveries-returns";

  allRecords : StoreOrder[] = [];
  deliveryList : StoreOrder[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  showSaveBtn : boolean;
  updateForm : FormGroup;

  delivery : any;
  isClient : boolean = false;

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
        this.defaultFilter = { DeliveryStatus : [2] };
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
    console.log(data);


    /*if(data["approvedDeliveryDate"] == null)
    {
      let approvedDeliveryDate = data["deliveryDate"].substring(0,10);

      this.updateForm = this.fb.group({
        id : new FormControl(data["id"]),
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
        
        approvedDeliveryDate = data["deliveryDate"].substring(0,10);
      }
      else{
        approvedDeliveryDate = data["approvedDeliveryDate"].substring(0,10);
        

      }

      this.updateForm = this.fb.group({
        id : new FormControl(data["id"]),
        approvedDeliveryDate : new FormControl(approvedDeliveryDate,Validators.required),
        driverName : new FormControl(data["driverName"],[Validators.required,Validators.maxLength(100)]),
        plateNumber : new FormControl(data["plateNumber"],[Validators.required,Validators.maxLength(10)]),
        deliveryStatus : new FormControl(data["deliveryStatus"]),
        transactionNo : new FormControl(data["transactionNo"])
      });
       
    }
    else {

      this.showSaveBtn = false;
      data["approvedDeliveryDate"] = data["approvedDeliveryDate"].substring(0,10);
      
    }

    this.delivery = data; 



  }

  toModel(detail : any): StoreOrder {
      let model = new StoreOrder({
        TransactionNo : detail.transactionNo,
        TransactionTypeStr : detail.returnTypeStr,
        ReturnFormNumber : detail.returnFormNumber,
        DRNumber : detail.drNumber,
        DeliverFrom : detail.returnedBy,
        DeliverTo : detail.deliverTo,
        DeliveryStatusStr : detail.deliveryStatusStr,
        DeliveryDate : (detail.deliveryDate != null) ? new Date(detail.deliveryDate).toLocaleString().slice(0,10).replace(",","") : '',
        ApprovedDeliveryDate : (detail.approvedDeliveryDate != null) ? new Date(detail.approvedDeliveryDate).toLocaleString().slice(0,10).replace(",","") : '',
        DeliveryQuantity : detail.deliveryQty
      });

      Object.getOwnPropertyNames(model).forEach(key => {
        model[key] = (model[key]) ? model[key] : '';
      });

      return model;
  }


  downloadList(){

    this._requestService.action = "deliveries/returns";
    var param = this.filterParam;
		param["showAll"] = true;
    
    var options = {
      fieldSeparator: ',',
      quoteStrings: '"',
      decimalseparator: '.',
      showLabels: true,
      headers: ['Transation No.','Transaction', 'RF No.', 'DR No.', 'Deliver From', 'Deliver To', 'Delivery Status', 'Requested Delivery Date', 'Approved Delivery Date', 'Delivery Qty']

        
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