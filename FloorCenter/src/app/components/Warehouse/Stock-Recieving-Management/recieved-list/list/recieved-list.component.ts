import { Component, AfterViewInit,OnInit,Input,ViewChild,ViewChildren} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { WarehouseInventoriesService } from '@services/warehouse/warehouse-inventories.service';
import { WarehouseInventories } from '@models/warehouse/warehouse-inventories.model';

import { PagerComponent } from '@pager/pager.component';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';
declare var $jquery: any;
declare var $: any;

@Component({
    selector: 'app-recieved-list',
    templateUrl: './recieved-list.html'
})

export class RecievedListComponent implements AfterViewInit {
     public now: Date = new Date();

  allRecords : WarehouseInventories[] = [];
  inventoryList:   WarehouseInventories[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;
  selectedInventoryid : number;
  selectedInventory : any;
  updateForm : FormGroup;

  module : string = "w-inventories";

    constructor(private _warehouseInvetoryService : WarehouseInventoriesService,
        private fb: FormBuilder) { }

  @ViewChild(PagerComponent)
  private pager: PagerComponent;

        loadAllInventories(pagerModel : any){

        this.allRecords = pagerModel["allRecords"];
        this.inventoryList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
    }

    onBtnViewDetailClick(data : any): void{
        this.selectedInventoryid = data.id;
        this.selectedInventory = data;
       

        this.updateForm = this.fb.group({


            id : new FormControl(data.id,Validators.required),

            deliveredItems : this.fb.array([])



        });
    }


      reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  } 



 downloadRecords(){
   var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction No.','Transaction', 'PO. No.', 'DR No.','Received Date']   
      };
      let title = this.now;
      let record = this.allRecords.map(r => this.toModel(r));

      new Angular2Csv(record, title.toISOString(), options);
  }



  toModel(detail : any): WarehouseInventories{
    let model = new WarehouseInventories({
 
     transactionNo: detail.transactionNo,
     warehouseId: detail.warehouseId,
    poNumber: detail.poNumber,
    drNumber: detail.drNumber,
    receivedDate: detail.receivedDate,
  
  
   
  

    });
    return model;
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