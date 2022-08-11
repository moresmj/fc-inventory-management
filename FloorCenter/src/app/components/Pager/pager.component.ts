import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

import { BaseService } from '@services/base.service';
import { StoreService } from '@services/store/store.service';
import { PagerService } from '@services/common/pager.service';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
    selector: 'app-pager',
    templateUrl: 'pager.html'
})

export class PagerComponent implements OnInit {



    @Input() module : string
    @Output() displayPageList : EventEmitter<any[]> = new EventEmitter<any[]>();
    pager: any = {};
    pagedItems: any[];
    listItems: any[];
    currentPage : number = 1;

    moduleService : any;
    private pagerModel : any[] = [];

    constructor(
        private _baseService : BaseService,
        private _storeService : StoreService,
        private _pagerService : PagerService) { }

    ngOnInit() {

        this.loadPageRecord();

    }

    loadPageRecord(page?: number) {

        this.moduleService = this._baseService.getService(this.module);
        this.moduleService.getList()
                        .subscribe(list => 
                            {
                                this.listItems  = list;

                                if(page != undefined)
                                {
                                    this.pager.totalPages = undefined;
                                    this.currentPage = 1;
                                }
                                this.setPage(this.currentPage);
                            });         
    }

    filterPageRecord(key: string) {

        this.moduleService = this._baseService.getService(this.module);
        this.moduleService.getList()
                        .subscribe(list => 
                            {
                                if (key != "") {
                                    this.listItems = list.filter(item => this.filterList(item,key));
                                }
                                else {
                                    this.listItems = list;
                                }                                
                                this.setPage(1);
                            });   
    }

    filterList(record : any, searchKey : string) : boolean {
        for(let key in record)
        {
            if (record[key] != null) {
                if(record[key].toString().toLowerCase().indexOf(searchKey.toLowerCase()) != -1)
                {
                    return true;
                }
            }
        }
        return false;
    }


    setPage(page: number) {

        this.currentPage = page;
        if (page < 1 || page > this.pager.totalPages) {
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
    
        // Return paginated list.
        this.displayPageList.emit(this.pagerModel);
    }


 
}