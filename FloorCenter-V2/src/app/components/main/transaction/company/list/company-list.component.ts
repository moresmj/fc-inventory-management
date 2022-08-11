import { Component, ViewChild, OnChanges, SimpleChanges, } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { Item } from '@models/item/item.model';

import { PageModuleService } from '@services/common/pageModule.service';
import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.html'
})

export class CompanyListComponent {

  userType : number;
  search : string;
  module : string = "company";

  allRecords : any[] = [];
  companyList : any[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  updateForm : FormGroup;

  selectedId : number;
  details : any;
  showAddPanel : boolean  = false;

  displayMessage : any;
  public now: Date = new Date();
  errorMessage : any;

  constructor(
      private fb: FormBuilder,
      private pageModuleService: PageModuleService) 
  {
      
  }

  ngOnInit() {

    this.userType = parseInt(this.pageModuleService.userType);
    this.errorMessage = null;

  }

  @ViewChild(PagerComponent)
  private pager : PagerComponent;


  addPanelShow(): void {
    this.showAddPanel = !this.showAddPanel;
  }

  getItems(pagerModel : any) {

        //this.showAddPanel = false;

        this.allRecords = pagerModel["allRecords"];
        this.companyList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        console.log(this.companyList);
  }

  reloadRecord(event : string){
    if(this.pager[event]){
      this.pager[event]();
    }
  }

  filterRecord() {

      if (this.search == undefined || this.search.trim() == "") {
        this.pager["loadPageRecord"](1);
        // this.errorMessage = "Please input search criteria";
     
      }
      else{
        this.pager["filterPageRecord"](this.search);
      }
         
  }

  onBtnUpdateClick(data : any) {


    this.showAddPanel = false;

    this.selectedId = data.id;
    this.details = data;
    this.updateForm = this.fb.group({
      Id : new FormControl(data.id),
      Code : new FormControl(data.code,Validators.required),
      Name : new FormControl(data.name,Validators.required),
    });
  }

  onSearch()
  {
    if (this.showAddPanel) {
      this.showAddPanel = false;
    }
  }

  toModel(detail : any): Item {
      let model = new Item({
        Id : detail.id,
        Code : detail.code,
        Name : detail.name,
        DateCreated : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString() : ''
      });

      return model;
  }

  
	onChange(ch : any){
  this.errorMessage = null;

  }


  downloadList(){
    
    var options = {
      fieldSeparator: ',',
      quoteStrings: '"',
      decimalseparator: '.',
      showLabels: true,
      headers: ['Company ID','Company Code', 'Company Name','Date Added']
        
    };
    let title = this.now;
    let record = this.allRecords.map(r => this.toModel(r));

    new Angular2Csv(record, title.toISOString(), options);
  }


}