import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

import { BaseService } from '@services/base.service';
import { PagerService } from '@services/common/pager.service';
import { CommonViewService } from '@services/common/common-view.service';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

@Component({
    selector: 'app-pagernew',
    templateUrl: './pagernew.html'
})

export class PagerNewComponent implements OnInit{

    template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">';

    @Input() module : string;
    @Output() displayPageList : EventEmitter<any[]> = new EventEmitter<any[]>();
    @Input() isDefaultFiltereredLoading : boolean = false;
    @Input() defaultNullLoading : boolean = false;
    @Input() defaultFilter : any;
    @Input() Keyword : any =[];

    defaultFilterNotYetLoaded = false;

    pager: any = {};
    pagedItems: any[];
    listItems: any[];
    currentPage : number = 1;
    errorMessage : any;
    params : any;

    customizedList : any = [ "release-items" ];
    

    moduleService : any;
    private pagerModel : any[] = [];

    constructor(
        private _baseService : BaseService,
        private _pagerService : PagerService,
        private _commonViewService: CommonViewService,
        private spinnerService: Ng4LoadingSpinnerService) { }

    ngOnInit() {
 
            this.loadPageRecord();   
    }
    loadnNull() {
        this.pagerModel["pageRecord"] = [];
        this.pagerModel["totalRecordMessage"] = `0 Records Found`;

        this.displayPageList.emit(this.pagerModel);
    }

    loadPageRecord(page?: number) {
        this.spinnerService.show();

        this.moduleService = this._baseService.getService(this.module);

        if(this.Keyword != null)
        {
           this.filterPageWithParams(1,this.Keyword);
        }else{
        this.moduleService.getList()
                        .subscribe(list => 
                            {
                                this.listItems  = list;

                                // Added because displaying of deliveries is complex and needed modification.

                                if (this.customizedList.indexOf(this.module) != - 1) {

                                    this.listItems = this.formatNewList(list["list"]);
                                }

                                if(page != undefined)
                                {
                                    this.pager.totalPages = undefined;
                                    this.currentPage = 1;
                                }
                                this.setPage(this.currentPage);
                                this.spinnerService.hide();
                            },
        error =>{

            this.spinnerService.hide();
        }
                            );   
    }      
    }

    filterPageRecord(key: string) {
        this.spinnerService.show();

        this.moduleService = this._baseService.getService(this.module);
        this.moduleService.getList()
                        .subscribe(list => 
                            {
                                if (key != "" && key != undefined) {
                                    this.listItems = list.filter(item => this.filterList(item,key));
                                }
                                else {
                                    this.listItems = list;


                                    if (this.customizedList.indexOf(this.module) != - 1) {

                                        this.listItems = this.formatNewList(list);
                                    }
                                }                                
                                this.setPage(1);
                                this.spinnerService.hide();
                            },
        error =>{
            this.spinnerService.hide();
        });   
    }


    filterPageRecordWithQueryString(key: string) {
       this.spinnerService.show();

        this.moduleService = this._baseService.getService(this.module);
        this.moduleService.getList()
                        .subscribe(list => 
                            {
                                if (key != "" && key != undefined) {
                                    this.listItems = list.filter(item => this.filterList(item,key));
                                }
                                else {
                                    this.listItems = list;


                                    if (this.customizedList.indexOf(this.module) != - 1) {

                                        this.listItems = this.formatNewList(list);
                                    }
                                }                                
                                this.setPage(1);
                               this.spinnerService.hide();
                            });   
    }


    filterList(record : any, searchKey : string) : boolean {
        for(let key in record)
        {
            if (record[key] != null) {

                if(typeof(record[key]) == "object" && key != 'importDetails')
                {
                    if(record[key]["name"].toString().toLowerCase().indexOf(searchKey.toLowerCase()) != -1)
                    {
                        return true;
                    }
                }
                else   
                {
                    if(key == 'transactionDate') {
                        if ((new Date(searchKey)).toLocaleDateString() == (new Date(record[key])).toLocaleDateString()) {
                            return true;
                        }
                    }
                    else if(record[key].toString().toLowerCase().indexOf(searchKey.toLowerCase()) != -1)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    



    filterPageWithParams(page : any, param : any) {
        var params;
        if(param != undefined)
        {
            // console.log("param has value");
          params = param;

        }
        else{
          params = this.Keyword;
          params["showAll"] = false;

        }
        params["currentPage"] = page;
        
        

        this.spinnerService.show();
        this.moduleService = this._baseService.getService(this.module);
        this.moduleService.getListWithParam(params)
                        .subscribe(list => 
                            {
                                this.listItems = list;

                                // if (this.customizedList.indexOf(this.module) != - 1) {
                                //     this.listItems['list'] = this.formatNewList(list['list']);
                                // }
                                
                               this.setPage(list["currentPage"]);
                                this.spinnerService.hide();

                            },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.pagerModel["errorMessage"] = this.errorMessage;
            this.displayPageList.emit(this.pagerModel);
            this.spinnerService.hide();
        });  

    }

    filterPageWithQueryParams(params : RequestOptions, key : string) {
        this.spinnerService.show();
        this.moduleService = this._baseService.getService(this.module);
        this.moduleService.getListWithParam(params)
                        .subscribe(list => 
                            {
                                // if (key != "" && key != undefined)  {
                                //     this.listItems = list.filter(item => this.filterList(item,key));
                                // }
                                // else {
                                    this.listItems = list;
                                // }                                
                                this.setPage(1);
                                this.spinnerService.hide();
                            },  
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.pagerModel["errorMessage"] = this.errorMessage;
            this.displayPageList.emit(this.pagerModel);
            this.spinnerService.hide();
        });  

    }

   


    setPage(page: number) {
       

        // console.log(this.listItems);
        page = this.listItems["currentPage"];
        this.currentPage = page;
        if (page < 1 || (page > this.listItems["totalPage"] && this.listItems["total"] == 0)) {
            return;
        } 

           
        // All records fetch before pagination.
        this.pagerModel["allRecords"] = this.listItems["list"];

        // get pager object from service
        this.pager = this._pagerService.getPager(this.listItems["total"], page);

        // get current page of items
        this.pagedItems = this.listItems["list"].slice(this.pager.startIndex, this.pager.endIndex + 1);

    //    console.log(this.pager); 


        this.pagerModel["pageRecord"] = this.listItems["list"];
        this.pagerModel["totalRecordMessage"] = `${this.pager["totalItems"]} Records Found`;

        if (this.pagerModel["pageRecord"].length > 0) {
            this.pagerModel["pageRecordMessage"] = `Displaying ( ${this.pager["startIndex"] + 1} - ${this.pager["endIndex"] + 1} ) records.`;
        }
        else{
            this.pagerModel["pageRecordMessage"] = null;
        }

         this.pagerModel["errorMessage"] = null;
    
        // Return paginated list.
        this.displayPageList.emit(this.pagerModel);
    }

    formatNewList(data : any) : any {

        let newList = [];

        for (let i = 0; i < data.length; i++ ) {

            // Array 1 - Can be DELIVERIES or ORDERED ITEMS
            let array1 = "";
            if ((data[i].orderType == 2) && (data[i].deliveryType == 1)) {
                //array1 = "orderedItems";
                array1 = "deliveries";
            }
            else {
               array1 = "deliveries";
            }

            let items = data[i][array1];

            if (array1 == "orderedItems") 
            {
                data[i]["details"] = data[i]; 
                data[i]["total"] = this.getTotalQuantity(items,"approvedQuantity"); 
                newList.push(data[i]);
            }
            else
            {
                for (let x = 0; x < items.length; x++) {

                    items[x]["details"] = data[i];

                    // Array 2 - Can be clientDeliveries or showroomDeliveries
                    let array2 = "";
                    if ((data[i].orderType == 2) && (data[i].deliveryType == 2)) 
                    {
                        array2 = "clientDeliveries";                    
                    }
                    else if ((data[i].orderType == 2) && (data[i].deliveryType == 1)) 
                    {
                        array2 = "clientDeliveries";                    
                    }
                    else
                    {
                        array2 = "showroomDeliveries"; 
                    }

                    items[x]["total"] = this.getTotalQuantity(items[x][array2],"quantity"); 
                    newList.push(items[x]);
                }
            }
        }
        return newList;
    }

    getTotalQuantity(data : any, qtyKey : string) : number {

        // qtyKey - can be quantity or approvedQuantity
        let total = 0;
        data.map(p => 
            { 
                total = total  + p[qtyKey];  
            });
    
        return total;

    }

    ngOnChanges(changes : SimpleChanges)
    {
        if (this.defaultFilterNotYetLoaded) {
            if(changes["defaultFilter"] != undefined)
            {
                if(changes["defaultFilter"].previousValue != changes["defaultFilter"].currentValue)
                {
                    this.filterPageWithParams(1,undefined);
                }
            }
        }       
    }

}