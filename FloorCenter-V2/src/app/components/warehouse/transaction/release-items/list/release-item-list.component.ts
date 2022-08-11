import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { ReleaseItems } from '@models/release-items/release-items.model';

import { ActivatedRoute, Router } from '@angular/router';

import { RequestService } from '@services/request.service';





import { PagerComponent } from '@common/pager/pager.component';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
declare var $jquery: any;
declare var $: any;

@Component({
    selector: 'app-release-item-list',
    templateUrl: './release-item-list.html'
})

export class ReleaseItemListComponent implements AfterViewInit {
     public now: Date = new Date();


 searchSuccess: any;
 errorMessage: any;
 statusMessage : any;

  allRecords : ReleaseItems[] = [];
  releaseItemList:   ReleaseItems[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;
  selectedInventoryid : number;
  selectedInventory : any;
  updateForm : FormGroup;
  searchForm : FormGroup;
  releaseItemDetails : any;
  forReleasingItems : any;
  forReleasingItems2 : any;
  items : any;
  defaultFilter : any;
  filter : any;
  isDefaultFiltereredLoading : boolean = true;
  test : boolean = false;

  Keyword : any = [];
  filterParam : any = [];



  module : string = "release-items";

  @ViewChild(PagerNewComponent)
  private pager: PagerNewComponent;

    constructor(private _releaseItemsService : ReleaseItemsService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private _requestService : RequestService,
        private _commonViewService : CommonViewService)
    {
        this.createSearchForm(); 


        this.route.queryParams.subscribe(
          data =>
          {
            this.searchForm.reset();

            for(let key in data)
            {
              this.searchForm.controls[key].setValue(data[key]);
              this.searchForm.controls[key].updateValueAndValidity(); 
            }

            if (data["deliveryType"] != undefined) {
                this.defaultFilter = this.searchForm.value;
                this.filter = this.searchForm.value 
                this.Keyword  = this.defaultFilter;
            }

    


          }

          );
    }



    loadAllReleaseItems(pagerModel : any){

        this.allRecords = pagerModel["allRecords"];
        this.releaseItemList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        this.errorMessage = pagerModel["errorMessage"];
console.log('test');
        console.log(this.releaseItemList);
    }

    onBtnReleaseClick(data : any): void{

      this.statusMessage = null;

      this.releaseItemDetails = data;
      this.items = data.orderedItems;
      let id = null;
      //added for ticket #211 [Client PO/Showroom Pickup] DR no. should be blank  
      this.releaseItemDetails["displayDr"] =  data.details.deliveryType == 3 && data.details.orderType == 2 ? false : true;
      this.releaseItemDetails["displaySiOr"] = data.details.deliveryType == 1 && data.details.orderType == 2 ? true : false;

      console.log(this.releaseItemDetails);
       if(data.details.deliveryType == 1)
        {
              id = data.details.id;
        }
        if((data.details.orderType == 2 && data.details.deliveryType == 2))
        {
           id =data.id;
        }
        else
        {

              id =data.id;
        }


          this.updateForm = this.fb.group({

           


              id: new FormControl(id),


          
            deliveryType: new FormControl(data.details.deliveryType),
            orderType : new FormControl(data.details.orderType),
            forReleasing : this.fb.array([])



          });

              if (data.details.orderType == 2 && data.details.deliveryType == 2) {
                 for(let i =0; i< this.releaseItemDetails.clientDeliveries.length; i++)
                          {


                           /* for(let x=0;x < this.releaseItemDetails.showroomDeliveries[i].forReleasing
                .length; x++)
                            {*/
                                       const control = <FormArray>this.updateForm.controls['forReleasing'];


                                       
                                       let item = this.releaseItemDetails["clientDeliveries"][i];
                                       this.forReleasingItems = this.releaseItemDetails["clientDeliveries"][i];
                                       this.forReleasingItems2 = this.releaseItemDetails["clientDeliveries"];
                                       console.log(this.forReleasingItems);
                                       let newItem = this.fb.group({

                                        code: new FormControl(item.item["code"]),
                                        name: new FormControl(item.item["name"]),
                                        tonality: new FormControl(item.item["tonality"]),
                                        description: new FormControl(item.item["description"]),




                                       })

                                       control.push(newItem);

                            //}
                          }
     
    }
    else {
      if(data.details.deliveryType != 1)
                { 
                  for(let i =0; i< this.releaseItemDetails.showroomDeliveries.length; i++)
                          {


                           /* for(let x=0;x < this.releaseItemDetails.showroomDeliveries[i].forReleasing
                .length; x++)
                            {*/
                                       const control = <FormArray>this.updateForm.controls['forReleasing'];


                                       
                                       let item = this.releaseItemDetails["showroomDeliveries"][i];
                                       this.forReleasingItems = this.releaseItemDetails["showroomDeliveries"][i];
                                       this.forReleasingItems2 = this.releaseItemDetails["showroomDeliveries"];
                                       console.log(this.forReleasingItems);
                                       let newItem = this.fb.group({

                                        code: new FormControl(item["code"]),
                                        name: new FormControl(item["name"]),
                                        tonality: new FormControl(item["tonality"]),
                                        description: new FormControl(item["description"]),




                                       })

                                       control.push(newItem);

                            //}
                          }

                }


                if(data.details.deliveryType == 1)
                { 
                  for(let i =0; i< this.releaseItemDetails.clientDeliveries.length; i++)
                          {


                           /* for(let x=0;x < this.releaseItemDetails.showroomDeliveries[i].forReleasing
                .length; x++)
                            {*/
                              const control = <FormArray>this.updateForm.controls['forReleasing'];


                                       
                                       let item = this.releaseItemDetails["clientDeliveries"][i];
                                       this.forReleasingItems = this.releaseItemDetails["clientDeliveries"][i];
                                       this.forReleasingItems2 = this.releaseItemDetails["clientDeliveries"];
                                       console.log(this.forReleasingItems);
                                       let newItem = this.fb.group({

                                        code: new FormControl(item.item["code"]),
                                        name: new FormControl(item.item["name"]),
                                        tonality: new FormControl(item.item["tonality"]),
                                        description: new FormControl(item.item["description"]),




                                       })

                                       control.push(newItem);

                            //}
                          }

                }
    
    }
                
                         



/*          this._releaseItemsService.getRecordWithId(data.id)
        .subscribe(order => {
          
        

        });*/
    

    
    }


    createSearchForm(){
        this.searchForm = this.fb.group({

            deliveryType : new FormControl('')

        });
    }

    clearMessage(){
      this.errorMessage = null;
    }

    onSearch(){
    let formData = this.searchForm.value;

    this.filter = formData;
    this.filter["currentPage"] = 1;
    this.Keyword = this.filter;
      // if (formData == "" && formData.length == 0) {
      //   this.pager["loadPageRecord"](1);
      // }
      // else {
         this.pager["filterPageWithParams"](1,formData);
      // }
    }

    onSearch2(data: any){

      
    this.pager["filterPageRecord2"](data);
    this.searchSuccess = true;
  
    }

    filterRecordWithParam(event : any) {
      
      this.filter = event;
      this.filter["currentPage"] = 1;
      this.Keyword = this.filter;
      this.pager["filterPageWithParams"](1,this.Keyword); 
      this.searchSuccess = true;    
  }


      reloadRecord(event : any) {

 //       this.statusMessage = event["statusMessage"];

    if (this.pager[event["pagerMethod"]]) {
      this.pager[event["pagerMethod"]]();    
    }
  } 



 downloadRecords(){
   var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction No.', 'Transaction','Order Type','DR No.','WHDR No.', 'Delivery Date', 'Plate No.','PO No.','Ordered By', 'Ordered To', 'Delivery Mode']   
      };

      this._requestService.action = "transactions/releaseitems";
      this.Keyword["showAll"] = true;
      var param = this.Keyword;
  
      this._requestService.getListWithParams(param)
              .subscribe(list =>{
                console.log(list);
                this.Keyword["showAll"] = false;
                  let record = list["list"].map(r => this.toModel(r));
                  let title = this.now;
                  new Angular2Csv(record, title.toISOString(), options);
                  
              },
              error =>{
                this.errorMessage = this._commonViewService.getErrors(error);
              });
  }



  toModel(detail : any): ReleaseItems{
    let model = new ReleaseItems({
      transaction_no : detail.details.transactionNo,
      transaction : detail.details.transactionTypeStr,
      orderType : detail.details.orderTypeStr,
      DRNumber:detail.drNumber,
      WHDRNumber:detail.details.whdrNumber,
      DeliveryDate: new Date(detail.deliveryDate).toLocaleString().slice(0,10).replace(",",""),
      PlateNumber:detail.plateNumber,
      po_num : detail.details.poNumber,
      ordered_by : detail.details.store.name,
      ordered_to : detail.details.warehouse.name,
      DeliveryType : detail.details.deliveryTypeStr,
    });

    Object.getOwnPropertyNames(model).forEach(key => {
      model[key] = (model[key]) ? model[key] : '';
    });

    return model;
  }


  onClickAdvanceSearch()
  {
        this.filter = this.searchForm.value;
        this.test = true;
  }



    async ngAfterViewInit() {
        $(document).ready(function(){
            $("#rDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("rDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        })
    }
}