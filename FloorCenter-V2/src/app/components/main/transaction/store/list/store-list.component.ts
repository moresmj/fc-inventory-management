import { Component,OnInit, AfterViewInit, ViewChild,EventEmitter,Output  } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { NgForm } from '@angular/forms';

import { PageModuleService } from '@services/common/pageModule.service';
import { PagerComponent } from '@common/pager/pager.component';
import { StoreService } from '@services/store/store.service';
import { Store } from '@models/store/store.model';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

import { CustomValidator } from '@validators/custom.validator';

declare var jquery:any;
declare var $ :any;


@Component({
	selector: 'app-store-list',
	templateUrl: './store-list.html'
})


export class StoreListComponent implements OnInit, AfterViewInit {

  userType : number;
  search : string;
  selectedStoreId : Number;
  selectedCode: string;
  public now: Date = new Date();
  @Output() removeMessage: EventEmitter<String> = new EventEmitter<String>();

  successMessage : string;
  errorMessage : any;

  updateForm : FormGroup;
  advanceSearchForm : FormGroup;

  showAddPanel : boolean  = false;

  allRecords : Store[] = [];
  storeList : Store[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  module : string = "store";

  @ViewChild(PagerComponent)
  private pager: PagerComponent;


  constructor(
        private _storeService: StoreService, 
        private fb: FormBuilder,
        private pageModuleService: PageModuleService) 
  {

  }

  ngOnInit() {
      this.userType = parseInt(this.pageModuleService.userType);
  }

  addPanelShow(): void {
    this.showAddPanel = !this.showAddPanel;
  }

  onBtnViewDetailClick(data : any): void {
    this.addPanelShow();

    this.selectedStoreId = data.id;
    this.selectedCode = data.code;
    this.updateForm = this.fb.group({
      Name : new FormControl(data.name,[Validators.required, Validators.maxLength(255)]),
      CompanyId : new FormControl(data.companyId,Validators.required),
      Address : new FormControl(data.address),
      ContactNumber :new FormControl(data.contactNumber,),
      WarehouseId : new FormControl(data.warehouseId,Validators.required)
    });

  }

  onBtnAdvanceSearchClick(): void {
    this.advanceSearchForm = this.fb.group({
      Name : new FormControl('')
    });
  }

  reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  }

  filterRecord() {

    if (this.search == "" && this.storeList.length == 0) {
      this.pager["loadPageRecord"](1);
    }
    else{
      this.pager["filterPageRecord"](this.search);
    }
       
  }

  onSearch()
  {
    if (this.showAddPanel) {
      this.showAddPanel = false;
    }

  }
 

  getStores(pagerModel : any): void {
        this.allRecords = pagerModel["allRecords"];
        this.storeList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
  }

  downloadStoreList() {
      var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['ID','Store Code','Store Name', 'Address', 'Contact No.', 'Company', 'Warehouse', 'Date Added']   
      };
      let title = this.now;
      let record = this.allRecords.map(r => this.toModel(r));

      new Angular2Csv(record, title.toISOString(), options);
  }

  toModel(detail : any): Store {
    let model = new Store({
      Id : detail.id,
      Code : detail.code,
      Name : detail.name,
      Address : detail.address,
      ContactNumber : detail.contactNumber,
      CompanyName : this.checker(detail.company),     
      WarehouseName : this.checker(detail.warehouse),
      DateCreated : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString() : ''
    });

    return model;
  }

  checker(data : any)
  {

  	if(data != null)
  	{
  		return data.name;
  	}
  	else{
  		return "";
  	}
  }

  onChange(ch : any){

    }


  async ngAfterViewInit() {

        $(function() {

          $(document).ready(function(){
              $(".addNew").click(function(){
                  $("#addItems").toggle();
              });
              $("#cancel").click(function(){
                  $("#addItems").toggle();
              });
              $("#poDate").daterangepicker({
              singleDatePicker: true,
              showDropdowns: true
              });
              $("#poDate2").daterangepicker({
                  singleDatePicker: true,
                  showDropdowns: true
              });
          });


          // We can attach the `fileselect` event to all file inputs on the page
          $(document).on('change', ':file', function() {
            var input = $(this),
                numFiles = input.get(0).files ? input.get(0).files.length : 1,
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
            input.trigger('fileselect', [numFiles, label]);
          });

          // We can watch for our custom `fileselect` event like this
          $(document).ready( function() {
              $(':file').on('fileselect', function(event, numFiles, label) {

                  var input = $(this).parents('.input-group').find(':text'),
                      log = numFiles > 1 ? numFiles + ' files selected' : label;

                  if( input.length ) {
                      input.val(log);
                  } else {
                      if( log ) alert(log);
                  }

              });
          });        
        });

  }


}