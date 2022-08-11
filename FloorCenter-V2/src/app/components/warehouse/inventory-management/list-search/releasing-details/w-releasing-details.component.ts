import { Component, Input , ViewChild } from '@angular/core';

import { ActivatedRoute } from '@angular/router';

import { RequestService } from '@services/request.service';

import { Inventories } from '@models/inventories/inventories.model';

import { PagerNewComponent } from '@common/pagernew/pagernew.component';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { BaseService } from '@services/base.service';
@Component({
    selector : 'app-w-releasing-details',
    templateUrl : './w-releasing-details.html'
})

export class WarehouseReleasingDetailsComponent {

    module : string = "w-releasing-details";

    public now: Date = new Date();

    filter : any;
    Keyword : any = [];

    allRecords : any = [];
    inventoryList : any = [];
    totalRecordMessage : string;
    pageRecordMessage : string;
    itemId : any;
    itemDetails : any;


    @Input() details : any;
    constructor(
        private _requestService : RequestService,
        private baseService : BaseService,
        private route: ActivatedRoute,
    ){
        this.itemId = route.snapshot.params['id'];
        this.baseService.param = this.itemId;
        

        this._requestService.action = "items"

        let itemFilter = { 'id' : this.itemId };
		this._requestService.getListWithParams(itemFilter).subscribe(data => { 
			this.itemDetails = data[0];
			console.log(this.itemDetails);
		} );


    }

    @ViewChild(PagerNewComponent)
    private pager : PagerNewComponent;

    getInventories(pagerModel : any) {
        this.allRecords = pagerModel["allRecords"];
        this.inventoryList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 


        console.log(this.allRecords);
  }

    filterRecordWithParam(formData : any) {
        this.filter = formData;
        this.filter["currentPage"] = 1;
        this.Keyword = this.filter;
        this.pager["filterPageWithParams"](1,this.Keyword); 
      }

      toModel(detail : any): Inventories {
        let model = new Inventories({
          ItemId : detail.poDate,
          SerialNumber : detail.poNumber,
          ItemName : detail.whdrNumber,  
          Code : detail.quantity,
        });
  
        return model;
    }



      downloadList(){

        this._requestService.action = "inventories/warehouse";
        this.Keyword["showAll"] = true;
        this.Keyword["keyword"] = (this.filter != undefined) ? this.filter : '';
        
        var options = {
          fieldSeparator: ',',
          quoteStrings: '"',
          decimalseparator: '.',
          showLabels: true,
          headers: ['ID','PO Date.','Po No,', 'DR No.', 'Quantity']

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


