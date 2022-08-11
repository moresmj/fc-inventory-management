import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { CommonViewService } from '@services/common/common-view.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { OrderStock } from '@models/order-stock/order-stock.model';

import { ActivatedRoute, Router } from '@angular/router';

import { PagerComponent } from '@common/pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { BaseComponent } from '@components/common/base.component';
import { RequestService } from '@services/request.service';

declare var $jquery: any;
declare var $: any;

@Component({

	selector: 'app-order-stock-list',
	templateUrl: 'order-stock-list.html'
})



export class OrderStockListComponent extends BaseComponent{

	   now: Date = new Date();
	  searchSuccess: any;
 	  errorMessage: any;

 	    allRecords : OrderStock[] = [];
  		orderStockList:   OrderStock[] = [];
  		totalRecordMessage : string;
  		pageRecordMessage : string;
  		selectedOrderid : number;
      selectedOrder : any;
      updateForm : FormGroup;
  		searchForm : FormGroup;
      addSiForm : FormGroup;

  		module : string = "order-stock";

      isDefaultFiltereredLoading : boolean = true;
      defaultFilter : any = { OrderStatus : ["2"]};
      filter : any ={ OrderStatus : ["2"], RequestStatus : ["1"], TransactionType : ["2"],DeliveryStatus:["3"] };
      fil : any; // filter number
      isFilter : any;
      currentDate: any = new Date().toISOString().substring(0,10);
      currentUser = JSON.parse(localStorage.getItem("currentUser")).userType
  	constructor(private _orderStockService : OrderStockService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private _requestService : RequestService,
        private _commonViewService : CommonViewService)
    {
        super();
        this.createSearchForm();
        this.Keyword = this.defaultFilter;
        this.route.queryParams.subscribe(
          data =>
          {
          
           this.fil = data.filter
            this.isFilter = data
            if(data.filter == 1){
              this.filter["filter"] = this.fil
              this.Keyword =this.filter
            
            }
            else if(data.filter == 0){
              this.defaultFilter["filter"] = this.fil
              this.Keyword =this.defaultFilter
              this.isDefaultFiltereredLoading = false
            }
            console.log(this.Keyword)
          });
          console.log(this.filter)
    }

     @ViewChild(PagerNewComponent)
  private pager: PagerNewComponent;

    loadAllReceiveItems(pagerModel : any){

        this.allRecords = pagerModel["allRecords"];
        this.orderStockList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        this.errorMessage = pagerModel["errorMessage"];

        console.log(this.orderStockList);
    }

    onBtnViewDetailClick(data : any): void{
        this.selectedOrderid = data.id;
        
        //ordered date is current date of submission
        if(data.poNumber == null){
          data.poDate = this.currentDate
        }
        this.selectedOrder = data;
        console.log(data);


        if(data.deliveryType == 1)
        {
          this.addSiForm  = this.fb.group({

            id : new FormControl(data.id, Validators.required),
            clientSINumber : new FormControl('',[Validators.required, Validators.maxLength(50)])

          });
        }

       

        this.updateForm = this.fb.group({


            id : new FormControl(data.id,Validators.required),
            deliveryType : new FormControl(data.deliveryType,Validators.required),
            paymentMode : new FormControl(data.paymentMode),
            poNumber : new FormControl(data.poNumber, this.currentUser == 6 ? Validators.required : null),
            poDate : new FormControl(this.currentDate),
            storeId : new FormControl(data.storeId),
            deliveredItems : this.fb.array([])
            


        });
    }




    CreateAoUpdateForm(){

    }

      createSearchForm(){
        this.searchForm = this.fb.group({


            transaction : new FormControl(),
            poNumber : new FormControl(),
            transactionNo : new FormControl(),
            poDateFrom : new FormControl(),
            poDateTo : new FormControl(),
            remarks : new FormControl()




        });
    }


    onSearch(){
    let formData = this.searchForm.value;


      // if (formData == "" && formData.length == 0) {
      //   this.pager["loadPageRecord"](1);
      // }

      this.Keyword = formData;
      this.Keyword["currentPage"] = 1;
      this.pager["filterPageWithParams"](1,this.Keyword); 

    // this.pager["filterPageWithParams"](formData);
      console.log(formData);
    }

    filterRecordWithParam(event : any) {

      
      this.Keyword = event;
      this.Keyword["currentPage"] = 1;
      this.pager["filterPageWithParams"](1,this.Keyword); 
      // this.pager["filterPageWithParams"](event);
      this.searchSuccess = true;    
  	}

  	reloadRecord(event : string) {
    	if (this.pager[event]) {
      	this.pager[event]();    
    	}
  	}



  	downloadRecords(){

      this._requestService.action = "transactions/orders";
            var options = {
                fieldSeparator: ',',
                quoteStrings: '"',
                decimalseparator: '.',
                showLabels: true,
                headers: ['ID', 'Transaction No.','Transaction', 'Order Type','Status', 'PO No.', 'Ordered Date',	'Ordered To', 'Order Status']   

                };
                
                var param = this.Keyword;
                param["showAll"] = true;
                
              this._requestService.getListWithParams(param)
                .subscribe(list =>{
                
                  let record = list["list"].map(r => this.toModel(r));
                  let title = this.now;
                  new Angular2Csv(record, title.toISOString(), options);
                },
                
              error =>{
                var errorMessage = this._commonViewService.getErrors(error);
                console.log(errorMessage);
              });
                // let title = this.now;
                // let record = this.allRecords.map(r => this.toModel(r));

                // new Angular2Csv(record, title.toISOString(), options);
            }



            toModel(detail : any): OrderStock{
                let model = new OrderStock({
             /*
                transactionNo: detail.transactionNo,
                warehouseId: detail.warehouseId,
                poNumber: detail.poNumber,
                drNumber: detail.drNumber,
                receivedDate: detail.receivedDate,
                */
    			     id: detail.id,
               transactionNo: detail.transactionNo,
               transactionTypeStr: detail.transactionTypeStr,
    			     orderTypeStr: detail.orderTypeStr,
    			     requestStatusStr : detail.requestStatusStr,
    			     poNumber : detail.poNumber,
    			     poDate : (detail.poDate != null) ? new Date(detail.poDate).toLocaleString().slice(0,10).replace(",","") : '',
               orderedTo : detail.orderedTo,
               orderStatusStr : detail.orderStatusStr,
               });
                return model;
  } 





}