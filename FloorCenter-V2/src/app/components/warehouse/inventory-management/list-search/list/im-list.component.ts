import { Component, ViewChild } from '@angular/core';

import { InventoriesService } from '@services/inventories/inventories.service';
import { Inventories } from '@models/inventories/inventories.model';

import { PagerComponent } from '@common/pager/pager.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { RequestService } from '@services/request.service';


@Component({
  selector: 'app-im-list',
  templateUrl: './im-list.html'
})

export class InventoryListComponent {

  search : string;
  module : string = "inventories";

  Keyword : any = [];
  filterParam : any = [];

  allRecords : Inventories[] = [];
  inventoryList : Inventories[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  searchForm : FormGroup;
  defaultFilter : any;
  filter : any;
  isDefaultFiltereredLoading : boolean = true;

  details : any;
  public now: Date = new Date();

  constructor(      
      private fb : FormBuilder,
      private _inventoriesService : InventoriesService,
      private route: ActivatedRoute,
      private router: Router,
      private _requestService : RequestService) 
  {
    this._inventoriesService.action = "warehouse";
    this.createForm();
    
    this.route.queryParams.subscribe(
      data => 
        { 
          for(let key in data) {
              this.searchForm.controls[key].setValue(true);
              this.searchForm.controls[key].updateValueAndValidity();  

              console.log(this.searchForm.value);
          }
          this.filter = this.searchForm.value;
          this.defaultFilter = this.searchForm.value;
          this.filter["currentPage"] = 1;
          this.Keyword = this.filter;
        }
    );
  }

  @ViewChild(PagerNewComponent)
  private pager : PagerNewComponent;


  getInventories(pagerModel : any) {
        this.allRecords = pagerModel["allRecords"];
        this.inventoryList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

        console.log(this.inventoryList)
  }

  reloadRecord(event : string){
    if(this.pager[event]){
      this.pager[event]();
    }
  }

  filterRecord() {
      // this.pager["filterPageWithQueryParams"](this.filter,this.search);
      this.Keyword["keyword"] = this.search;
      this.pager["filterPageWithParams"](1,this.Keyword);          
  }

  filterRecordWithParam(formData : any) {
    this.filter = formData;
    this.filter["currentPage"] = 1;
    this.filter["keyword"] = this.search;
    this.Keyword = this.filter;
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
        OnHand : detail.onHand,
        ForRelease : detail.forRelease,  
        Broken : detail.broken,    
        Available : detail.available       
      });

      return model;
  }

  createForm() {
    this.searchForm = this.fb.group({
      OnlyAvailableStocks : new FormControl(false),
      IsOutOfStocks : new FormControl(false),
      HasBroken : new FormControl(false)
    });
  }

  onBtnReleasingDetailsClick(event : any){
    this._requestService.action = "inventories/release/details/";



  }

  downloadList(){

    this._requestService.action = "inventories/warehouse";
    this.Keyword["showAll"] = true;
    this.Keyword["keyword"] = (this.search != undefined) ? this.search : '';
    
    var options = {
      fieldSeparator: ',',
      quoteStrings: '"',
      decimalseparator: '.',
      showLabels: true,
      headers: ['ID','Serial No.','Item Code', 'Item Name', 'Size', 'Tonality', 'On-Hand', 'For Release','Broken', 'Available']
        
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