import { Component, ViewChild } from '@angular/core';

import { InventoriesService } from '@services/inventories/inventories.service';
import { Inventories } from '@models/inventories/inventories.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
  selector: 'app-ai-list',
  templateUrl: './ai-list.html'
})

export class AdjustInventoryListComponent {

  search : string;
  module : string = "adjust-inventory";

  isDefaultLoad : boolean = true;
  filterParam : any;

  allRecords : Inventories[] = [];
  inventoryList : Inventories[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;
  



  details : any;
  public now: Date = new Date();

  constructor(private _inventoriesService : InventoriesService) 
  {

  }

  @ViewChild(PagerComponent)
  private pager : PagerComponent;


  getInventories(pagerModel : any) {

    if (this.isDefaultLoad) {
      let defaultFilter = { "RequestStatus" : [2] };
      this.filterRecordWithParam(defaultFilter);

      this.isDefaultLoad = false;
    }
    else{
      this.allRecords = pagerModel["allRecords"];
      this.inventoryList =  pagerModel["pageRecord"]; 
      this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
      this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
    }
 
       

        console.log(this.inventoryList);
  }


  filterRecordWithParam(event : any) {

		if (event == "loadPageRecord") {
	    	this.pager["filterPageWithParams"](this.filterParam);  
		}
		else
		{
			this.filterParam = event;
		    this.pager["filterPageWithParams"](event);  
		}
  
	}

  reloadRecord(event : string){
    if(this.pager[event]){
      this.pager[event]();
    }
  }

  filterRecord() {

      if (this.search == "" && this.inventoryList.length == 0) {
        this.pager["loadPageRecord"](1);
      }
      else{
        this.pager["filterPageRecord"](this.search);
      }
         
  }



  toModel(detail : any): Inventories {
      let model = new Inventories({
        ItemId : detail.store,
        StoreAddress : (detail.dateUploaded != null) ? new Date(detail.dateUploaded).toLocaleString().slice(0,10).replace(",","") : '',
        Code : detail.requestStatusStr,
      
      });

      return model;
  }


  downloadList(){
    
    var options = {
      fieldSeparator: ',',
      quoteStrings: '"',
      decimalseparator: '.',
      showLabels: true,
      headers: ['Store','Date Uploaded','Status']
        
    };
    let title = this.now;
    let record = this.allRecords.map(r => this.toModel(r));

    new Angular2Csv(record, title.toISOString(), options);
  }

}