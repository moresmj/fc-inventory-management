import { Component, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';


import { InventoriesService } from '@services/inventories/inventories.service';
import { Inventories } from '@models/inventories/inventories.model';

import { PagerComponent } from '@common/pager/pager.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import 'rxjs/add/operator/map';

import { RequestService } from '@services/request.service';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
  selector: 'app-pcs-im-list',
  templateUrl: './pcs-im-list.html'
})

export class PhysicalCountSummaryListComponent {

  search : string;
  module : string = "inventories";

  Keyword : any = [];

  allRecords : Inventories[] = [];
  inventoryList : Inventories[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;



  details : any;
  public now: Date = new Date();
  pipe = new DatePipe('en-US');

  constructor(private _inventoriesService : InventoriesService,
    private _requestService : RequestService) 
  {
    this._inventoriesService.action = "warehouse/physicalcount/summary";
  }

  @ViewChild(PagerNewComponent)
  private pager : PagerNewComponent;


  getInventories(pagerModel : any) {
        this.allRecords = pagerModel["allRecords"];
        this.inventoryList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

        console.log(this.inventoryList);
  }

  reloadRecord(event : string){
    if(this.pager[event]){
      this.pager[event]();
    }
  }

  filterRecord() {

    this.Keyword["keyword"] = this.search;
    this.pager["filterPageWithParams"](1,this.Keyword);   
         
  }

  onBtnUpdateClick(data : any) {
    this.details = data;
  }


  toModel(detail : any): Inventories {
      let model = new Inventories({
        ItemId : detail.itemId,
        SerialNumber : detail.serialNumber,
        Code : detail.code,
        ItemName : detail.itemName,
        SizeName : detail.sizeName,
        Tonality : detail.tonality,
        OnHand : new Date(detail.dateCreated).toLocaleString(),
        ForRelease : detail.physicalCount,
        Broken : detail.adjustment     
      });

      return model;
  }


  downloadList(){

    this._requestService.action = "inventories/warehouse/physicalcount/summary";
    this.Keyword["showAll"] = true;
    this.Keyword["keyword"] = (this.search != undefined) ? this.search : '';

    var options = {
      fieldSeparator: ',',
      quoteStrings: '"',
      decimalseparator: '.',
      showLabels: true,
      headers: ['ID','Serial No.','Item Code', 'Item Name', 'Size', 'Tonality', 'Last Date Counted', 'Physical Count', 'Adjustment']
        
    };
    

    this._requestService.getListWithParams(this.Keyword)
            .subscribe(list =>{
              console.log(list);
                this.Keyword["showAll"] = false;
                let record = list["list"].map(r => this.toModel(r));
                let title = this.now;
                new Angular2Csv(record, title.toISOString(), options);
           
                
            },
            error =>{
              // this.errorMessage = this._commonViewService.getErrors(error);
            });
  }

}