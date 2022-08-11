import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { Returns } from '@models/returns/returns.model';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { RequestService } from '@services/request.service';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
    selector : 'app-re-cr-list',
    templateUrl: 're-cr-list.html'
})



export class ClientReturnListComponent{

    module : string = "clientreturn";

    searchForm : FormGroup;

    errorMessage: any;
    statusMessage : any;

    details : any;

    Keyword : any = [];
    filterParam : any = [];
    filter : any;

    allRecords : any = [];
    returnList : any = [];
    totalRecordMessage : string;
    pageRecordMessage : string;

    public now: Date = new Date();

    constructor(private fb : FormBuilder,
        private _requestService : RequestService)
    {

        this.createForm();

    }

    @ViewChild(PagerNewComponent)
    private pager : PagerNewComponent;


    getReturns(pagerModel : any) {

        this.allRecords = pagerModel["allRecords"];
        this.returnList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 

        console.log(this.allRecords);
        console.log("test");
    }

    reloadRecord(event : string) {
        if(this.pager[event]){
            this.pager[event]();
        }
    }


    onSearch(){
        let formValue = this.searchForm.value;


        this.filterRecordWithParam(formValue);
    }

    filterRecordWithParam(event : any) {

        this.filter = event;
        this.filter["currentPage"] = 1;
        this.Keyword = this.filter;
        this.pager["filterPageWithParams"](1,this.Keyword);    
    }


    onBtnDetailsClick(data : any) {
        this.details = data;
    }

    toModel(detail : any): Returns {
        let model = new Returns({
          transactionNo : detail.transactionNo,
          returnFormNumber : detail.salesTypeStr,
          returnTypeStr : detail.siNumber,
          orNumber : detail.orNumber,
          requestDate : (detail.releaseDate != null) ? detail.releaseDate.slice(0,10) : '',
          requestStatusStr : detail.clientName,

        });

        return model;
    }

    downloadList(){
        
        var options = {
            fieldSeparator: ',',
            quoteStrings: '"',
            decimalseparator: '.',
            showLabels: true,
            headers: ['Transaction No.','Transaction', "SI No.", "OR No.", "Release  Date" , "Customer Name"]               
        };
;


        this._requestService.action = "returns/clientreturn";
        this.Keyword["showAll"] = true;
        
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


        createForm() {
        this.searchForm = this.fb.group({
            SINumber : new FormControl(''),
            ORNumber : new FormControl(''),
            ClientName : new FormControl('')
        });
    }


}
