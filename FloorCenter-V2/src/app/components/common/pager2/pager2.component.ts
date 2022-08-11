import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

import { BaseService } from '@services/base.service';
import { PagerService } from '@services/common/pager.service';
import { CommonViewService } from '@services/common/common-view.service';


@Component({
    selector: 'app-pager2',
    templateUrl: './pager2.html'
})

export class Pager2Component implements OnInit {

    @Output() displayPageList : EventEmitter<any[]> = new EventEmitter<any[]>();
   
    pager: any = {};
    pagedItems: any[];
    listItems: any[];

    public currentPage : number = 1;
    errorMessage : any;

    private pagerModel : any[] = [];

    constructor(
        private _baseService : BaseService,
        private _pagerService : PagerService,
        private _commonViewService: CommonViewService) { }

    ngOnInit() {

    }

    setItemPageRecord(data : any) {

        this.listItems  = data;
        this.setPage(this.currentPage);
         
    }

    filterPageRecord(data : any, filter: any) {

        this.listItems = data.filter(item => this.filterList(item,filter));                            
        this.setPage(1);

    }

    filterList(item : any, filter : any) : boolean {

        var isNullSearching = true;

        for(let key in filter)
        {
            if (filter[key] != "") {

                isNullSearching = false;

                let value = filter[key].toLowerCase();
                if (item[key].toString().toLowerCase().indexOf(value) != -1) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        return isNullSearching;
    }


    setPage(page: number) {

        this.currentPage = page;
        if (page < 1 || (page > this.pager.totalPages && this.listItems.length == 0)) {
            return;
        }      

        // All records fetch before pagination.
        this.pagerModel["allRecords"] = this.listItems;

        // get pager object from service
        this.pager = this._pagerService.getPager(this.listItems.length, page);

        // get current page of items
        this.pagedItems = this.listItems.slice(this.pager.startIndex, this.pager.endIndex + 1);


        this.pagerModel["pageRecord"] = this.pagedItems;
        this.pagerModel["totalRecordMessage"] = `${this.pager["totalItems"]} Records Found`;

        if (this.pagedItems.length > 0) {
            this.pagerModel["pageRecordMessage"] = `Displaying ( ${this.pager["startIndex"] + 1} - ${this.pager["endIndex"] + 1} ) records.`;
        }
        else{
            this.pagerModel["pageRecordMessage"] = null;
        }

         this.pagerModel["errorMessage"] = null;
    
        // Return paginated list.
        this.displayPageList.emit(this.pagerModel);
    }

 
}