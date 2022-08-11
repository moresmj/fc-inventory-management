import { Component, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { Item } from '@models/item/item.model';

import { PageModuleService } from '@services/common/pageModule.service';
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';
import { CustomValidator } from '@validators/custom.validator';

@Component({
  selector: 'app-item-list',
  templateUrl: './item-list.html'
})

export class ItemListComponent {

  userType : number;
  search : string;
  module : string = "items2";

  allRecords : Item[] = [];
  itemList : Item[] = [];
  totalRecordMessage : string;
  pageRecordMessage : string;

  updateForm : FormGroup;

  selectedId : number;
  details : any;
  itemStatus: any;
  showAddPanel : boolean  = false;

  displayMessage : any;

  errorMessage : any;

  Keyword : any = [];
  public now: Date = new Date();

  constructor(
      private fb: FormBuilder,
      private pageModuleService: PageModuleService,
      private _requestService : RequestService,
      private _commonViewService : CommonViewService) 
  {
      
  }

  ngOnInit() {

    this.userType = parseInt(this.pageModuleService.userType);

  }

  @ViewChild(PagerNewComponent)
  private pager : PagerNewComponent;


  addPanelShow(): void {
    this.showAddPanel = !this.showAddPanel;
  }

  getItems(pagerModel : any) {

        //this.showAddPanel = false;

        this.allRecords = pagerModel["allRecords"];
        this.itemList =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
        console.log(this.itemList);
  }

  reloadRecord(event : string){
    if(this.pager[event]){
      this.pager[event]();
    }
  }

  filterRecord() {

      // if (this.Keyword == "" && this.itemList.length == 0) {
      //   this.pager["loadPageRecord"](1);
      // }
      // else{
        this.Keyword["keyword"] = this.search;
        this.pager["filterPageWithParams"](1,this.Keyword);
      // }
         
  }

  onBtnUpdateClick(data : any) {
    console.log(data);


    this.showAddPanel = false;

    this.selectedId = data.id;
    this.details = data;
    this.itemStatus = data.isActive;
    this.updateForm = this.fb.group({
      Id : new FormControl(data.id),
      SerialNumber : new FormControl(data.serialNumber),
      Code : new FormControl(data.code,[Validators.required]),
      Name : new FormControl(data.name,[Validators.required]),
      SRP : new FormControl(data.srp,[Validators.required, CustomValidator.SRP,Validators.maxLength(16)]),
      Description : new FormControl(data.description),
      SizeId : new FormControl(data.sizeId,Validators.required),
      ImageName : new FormControl(data.imageName),
      Tonality : new FormControl(data.tonality,[Validators.required, CustomValidator.shouldNotContainDot]),
      QtyPerBox: new FormControl(data.qtyPerBox, [Validators.required, CustomValidator.shouldNotContainDot, CustomValidator.requestedQuantity]),
      BoxPerPallet: new FormControl(data.boxPerPallet,[Validators.required, CustomValidator.shouldNotContainDot, CustomValidator.requestedQuantity]),
      Cost: new FormControl(data.cost,[Validators.required, CustomValidator.Cost,Validators.maxLength(16)]),
      IsActive: new FormControl(data.isActive,[Validators.required]),
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
        SerialNumber : detail.serialNumber,
        Code : detail.code,
        Name : detail.name,
        Description : (detail.description != null) ? detail.description : '',
        SizeName : (detail.size != null) ? detail.size.name : '',
        SRP : detail.srp,
        Cost : detail.cost,
        Tonality : detail.tonality,
        QtyPerBox : detail.qtyPerBox,
        QtyPerPallet : detail.boxPerPallet,
        DateCreated : (detail.dateCreated != null) ? new Date(detail.dateCreated).toLocaleString() : ''
      });

      return model;
  }


  downloadList(){

    this._requestService.action = "items/items2";

    var options = {
        fieldSeparator: ',',
        quoteStrings: '"',
        decimalseparator: '.',
        showLabels: true,
        headers: ['ID','Serial No.','Item Code', 'Item Name', 'Description', 'Size','SRP','Cost',  'Tonality', 'QtyPerBox', 'QtyPerPallet',  'Date Added']
          
      };


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

}