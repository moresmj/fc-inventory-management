import { Component,ViewChild } from '@angular/core';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import { FormBuilder, FormGroup, FormControl, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';
import { InventoriesService } from '@services/inventories/inventories.service';
import { CookieService } from 'ngx-cookie-service';

import { BaseComponent } from '@components/common/base.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';


import { ReleaseItems } from '@models/release-items/release-items.model';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';


@Component({
    selector: 'app-change-tonality-list',
    templateUrl: './change-tonality-list.html'

})


export class ChangeTonalityListComponent extends BaseComponent{
  template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">'

    module : string = "change-tonality";
    releaseItemDetails : any;
    items : any;
    updateForm : FormGroup;
    forReleasingItems : any;
    forReleasingItems2 : any;
    itemList: any;
    filteredItemList : any;

    @ViewChild(PagerNewComponent)
    private pager: PagerNewComponent;

    constructor(private fb: FormBuilder,
        private route : ActivatedRoute,
        private _requestService : RequestService,
        private _commonViewService : CommonViewService,
        private _inventoriesService : InventoriesService,
        private _cookieService : CookieService,
        private spinnerService: Ng4LoadingSpinnerService,)
    {
        super();
        this.load();
    }



    loadAllForChangeTonality(pagerModel : any)
    {
        this.allRecords = pagerModel["allRecords"];
        this.recordList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        this.errorMessage = pagerModel["errorMessage"];

        console.log(this.recordList);
    }

    filterRecordWithParam(event : any) {
      
        this.Keyword = event;
        this.Keyword["currentPage"] = 1;
        this.pager["filterPageWithParams"](1,this.Keyword); 
        this.searchSuccess = true;    
    }

    reloadRecord(event : any) {

      //       this.statusMessage = event["statusMessage"];
     
         if (this.pager[event["pagerMethod"]]) {
           this.pager[event["pagerMethod"]]();    
         }
       } 


    load()
    {
        this._inventoriesService.action = "warehouse/items";
        this._requestService.action = "inventories/warehouse/items";
        // this.spinnerService.show();

        var warehouseId = this._cookieService.get('warehouseId');

    //     this._inventoriesService.getListWithID(parseInt(warehouseId)).subscribe( data => {
    //     this.itemList = data;
    //     this.spinnerService.hide();
    //     console.log(data);

    // });
        this._requestService.getRecordWithID(parseInt(warehouseId)).subscribe( data => {
          this.itemList = data;
          // this.spinnerService.hide();
          console.log(data);

          });
    }


    onBtnReleaseClick(data : any): void{

        this.statusMessage = null;
  
        this.releaseItemDetails = data;
        this.items = data.orderedItems;
        let id = null;
        //added for ticket #211 [Client PO/Showroom Pickup] DR no. should be blank  
        this.releaseItemDetails["displayDr"] =  data.details.deliveryType == 3 && data.details.orderType == 2 ? false : true;
        this.releaseItemDetails["displaySiOr"] = data.details.deliveryType == 1 && data.details.orderType == 2 ? true : false;
  
        console.log(this.releaseItemDetails);
         if(data.details.deliveryType == 1)
          {
                id = data.details.id;
          }
          if((data.details.orderType == 2 && data.details.deliveryType == 2))
          {
             id =data.id;
          }
          else
          {
  
                id =data.id;
          }

          var itemCodes : any = [];

          var showroomDel  = data.showroomDeliveries.map(p => p.item.code.trim());
          var clientDel  = data.clientDeliveries.map(p => p.item.code.trim());

          itemCodes = itemCodes.concat(showroomDel);
          itemCodes = itemCodes.concat(clientDel);
          console.log(itemCodes);

         this.filteredItemList = this.itemList.filter(p => itemCodes.includes(p.code.trim()));
  
  
            this.updateForm = this.fb.group({

              STOrderId: new FormControl(data.details.id),
              deliveryType: new FormControl(data.details.deliveryType),
              orderType : new FormControl(data.details.orderType),
              remarks: new FormControl(),
              ModifyItemTonalityDetails : this.fb.array([])
            });
  
                if (data.details.orderType == 2 && data.details.deliveryType == 2) {
                   for(let i =0; i< this.releaseItemDetails.clientDeliveries.length; i++)
                                        {
  
                                         const control = <FormArray>this.updateForm.controls['ModifyItemTonalityDetails'];
                                         let item = this.releaseItemDetails["clientDeliveries"][i];
                                         this.forReleasingItems = this.releaseItemDetails["clientDeliveries"][i];
                                         this.forReleasingItems2 = this.releaseItemDetails["clientDeliveries"];
                                         console.log(this.forReleasingItems);
                                         let newItem = this.fb.group({

  
                                          // code: new FormControl(item.item["code"]),
                                          // name: new FormControl(item.item["name"]),
                                          // tonality: new FormControl(item.item["tonality"]),
                                          // description: new FormControl(item.item["description"]),
                                          itemId : new FormControl(),
                                          oldItemId : new FormControl(item.item["id"]),
                                          requestedQty : new FormControl(item.quantity),
                                          warehouseId : new FormControl(data.details.warehouseId),
                                          stClientDeliveryId: new FormControl(item.id),
                                          remarks : new FormControl(),
                                          
                                          
  
  
  
  
                                         })
  
                                         control.push(newItem);
  
                  
                            }
       
      }
      else {
        if(data.details.deliveryType != 1)
                  { 
                    for(let i =0; i< this.releaseItemDetails.showroomDeliveries.length; i++)
                            {

                                         const control = <FormArray>this.updateForm.controls['ModifyItemTonalityDetails'];

                                      

                                         let item = this.releaseItemDetails["showroomDeliveries"][i];
                                         this.forReleasingItems = this.releaseItemDetails["showroomDeliveries"][i];
                                         this.forReleasingItems2 = this.releaseItemDetails["showroomDeliveries"];

                                         console.log(this.forReleasingItems);
                                         let newItem = this.fb.group({
  
                                          itemId : new FormControl(),
                                          oldItemId : new FormControl(item.item["id"]),
                                          requestedQty : new FormControl(item.quantity),
                                          warehouseId : new FormControl(data.details.warehouseId),
                                          stShowroomDeliveryId: new FormControl(item.id),
                                          remarks : new FormControl()
                                         })
  
                                         control.push(newItem);

                            }
  
                  }
  
  
                  if(data.details.deliveryType == 1)
                  { 
                    for(let i =0; i< this.releaseItemDetails.clientDeliveries.length; i++)
                            {
  
                                const control = <FormArray>this.updateForm.controls['ModifyItemTonalityDetails'];

                                         let item = this.releaseItemDetails["clientDeliveries"][i];
                                         this.forReleasingItems = this.releaseItemDetails["clientDeliveries"][i];
                                         this.forReleasingItems2 = this.releaseItemDetails["clientDeliveries"];
                                         console.log(this.forReleasingItems);
                                         let newItem = this.fb.group({
  
                                          itemId : new FormControl(),
                                          oldItemId : new FormControl(item.item["id"]),
                                          requestedQty : new FormControl(item.quantity),
                                          warehouseId : new FormControl(data.details.warehouseId),
                                          stClientDeliveryId: new FormControl(item.id),
                                          remarks : new FormControl()
                                    
  

                                         })
  
                                         control.push(newItem);
                                         
  

                            }
  
                  }
      
      }
                  
      
      }


    
 downloadRecords(){
    var options = {
        fieldSeparator: ',',
        quoteStrings: '"',
        decimalseparator: '.',
        showLabels: true,
        headers: ['Transaction No.', 'Transaction','Order Type','DR No.','WHDR No.', 'Delivery Date', 'Plate No.','PO No.','Ordered By', 'Ordered To', 'Delivery Mode']   
       };
 
       this._requestService.action = "transactions/releaseitems/changetonality";
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
 
 
 
   toModel(detail : any): ReleaseItems{
     let model = new ReleaseItems({
       transaction_no : detail.details.transactionNo,
       transaction : detail.details.transactionTypeStr,
       orderType : detail.details.orderTypeStr,
       DRNumber:detail.drNumber,
       WHDRNumber:detail.details.whdrNumber,
       DeliveryDate: new Date(detail.deliveryDate).toLocaleString().slice(0,10).replace(",",""),
       PlateNumber:detail.plateNumber,
       po_num : detail.details.poNumber,
       ordered_by : detail.details.store.name,
       ordered_to : detail.details.warehouse.name,
       DeliveryType : detail.details.deliveryTypeStr,
     });
 
     Object.getOwnPropertyNames(model).forEach(key => {
       model[key] = (model[key]) ? model[key] : '';
     });
 
     return model;
   }


}