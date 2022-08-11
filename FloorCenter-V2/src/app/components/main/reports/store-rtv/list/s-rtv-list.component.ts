import { Component,OnInit, AfterViewInit, ViewChild,EventEmitter  } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { NgForm } from '@angular/forms';

import { PageModuleService } from '@services/common/pageModule.service';
import { PagerComponent } from '@common/pager/pager.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';

import { StoreRTVReport } from '@models/store/store-rtv-report.model';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { CustomValidator } from '@validators/custom.validator';

import { RequestService } from '@services/request.service';

declare var jquery:any;
declare var $ :any;


@Component({
	selector: 'app-s-rtv-list',
	templateUrl: './s-rtv-list.html'
})


export class StoreRTVListComponent implements OnInit, AfterViewInit {


  public now: Date = new Date();

  advanceSearchForm : FormGroup;

  allRecords : any = [];
  recordList : any = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  filterParam : any = [];
  defaultNullLoading  : boolean = true;
  module : string = "main-store-rtv";

  Keyword : any = [];


  @ViewChild(PagerNewComponent)
  private pager: PagerNewComponent;


  constructor(
        private fb: FormBuilder,
        private pageModuleService: PageModuleService,
        private _requestService : RequestService) 
  {

  }

  ngOnInit() {

  }


  reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  }

  filterRecordWithParam(event : any) {

    // if (event == "clear") {
    //     this.pager["loadnNull"]();  
    // }
    // else
    // {
      this.filterParam = event;
      this.filterParam["currentPage"] = 1;
      this.Keyword = this.filterParam;
      this.pager["filterPageWithParams"](1,this.Keyword);  
    // }
  
  }

  getRecords(pagerModel : any): void {
        this.allRecords = pagerModel["allRecords"];
        this.recordList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
  }

  downloadStoreList() {

    this._requestService.action = "returns/rtv/main";
    this.Keyword["showAll"] = true;



      var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['RF No.','DR No.','Item Code','Size', 'Good', 'Broken']   
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
      // let title = this.now;
      // let record = this.allRecords.map(r => this.toModel(r));

      // new Angular2Csv(record, title.toISOString(), options);
  }

  toModel(detail : any): StoreRTVReport {
    let model = new StoreRTVReport({
      returnFormNumber : detail.returnFormNumber,
      drNumber : detail.drNumber,
      code : detail.code,
      sizeName : detail.sizeName,
      receivedGoodQuantity : detail.receivedGoodQuantity,
      receivedBrokenQuantity : detail.receivedBrokenQuantity
    });

    return model;
  }


  async ngAfterViewInit() {

        $(function() {

       
        });

  }


}