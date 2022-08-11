import { Component, ViewChild } from "@angular/core";
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
  NgModel
} from "@angular/forms";

import { StoreOrder } from "@models/store-order/store-order.model";

import { PagerComponent } from "@common/pager/pager.component";
import { PagerNewComponent } from '@common/pagernew/pagernew.component';
import "rxjs/add/operator/map";

import { Angular2Csv } from "angular2-csv/Angular2-csv";

import { RequestService } from "@services/request.service";
import { CommonViewService } from '@services/common/common-view.service';

@Component({
  selector: "app-assign-dr-list",
  templateUrl: "./assign-dr-list.html"
})
export class AssignDrListComponent {
  module: string = "assign-dr";

  allRecords: StoreOrder[] = [];
  deliveryList: StoreOrder[] = [];
  totalRecordMessage: string;
  pageRecordMessage: string;

  keyWord : any = [];

  showSaveBtn: boolean;
  updateForm: FormGroup;

  isClient: boolean = false;
  delivery: any;

  public now: Date = new Date();

  constructor(private fb: FormBuilder,
    private _requestService : RequestService,
    private _commonViewService : CommonViewService) {}

  @ViewChild(PagerNewComponent)
  private pager: PagerNewComponent;

  getDeliveries(pagerModel: any) {
    this.allRecords = pagerModel["allRecords"];
    this.deliveryList = pagerModel["pageRecord"];
    this.totalRecordMessage = pagerModel["totalRecordMessage"];
    this.pageRecordMessage = pagerModel["pageRecordMessage"];

    console.log(this.deliveryList);
  }

  reloadRecord(event: string) {
    if (this.pager[event]) {
      this.pager[event]();
    }
  }

  filterRecordWithParam(event: any) {
    this.keyWord = event;
    this.pager["filterPageWithParams"](1,this.keyWord);
  }

  onBtnUpdateClick(detail: any) {
    this.showSaveBtn = true;

    this.updateForm = this.fb.group({
      id: new FormControl(detail.id),
      WHDRNumber: new FormControl("", Validators.required),
      transactionNo: new FormControl(detail.transactionNo)
    });

    this.delivery = detail; 
    this.updateForm = this.fb.group({
      id : new FormControl(detail.id),
      WHDRNumber : new FormControl('',[Validators.required, Validators.maxLength(50)]),
      transactionNo : new FormControl(detail.transactionNo)
    });


    if (detail.whdrNumber != null) {
      this.showSaveBtn = false;
    }
  }

  toModel(detail: any): StoreOrder {
    let model = new StoreOrder({
      TransactionNo: detail.transactionNo,
      TransactionTypeStr: detail.orderTypeStr,
      DeliveryStatusStr: detail.requestStatusStr,
      store: detail.orderedBy,
      warehouse: detail.orderedTo,
      PONumber: detail.poNumber,
      PODate: (detail.poDate != null) ? new Date(detail.poDate).toLocaleString().slice(0, 10).replace(",", "") : "",
      OrderedDate: (detail.orderedDate != null) ? new Date(detail.orderedDate).toLocaleString().slice(0, 10).replace(",", "") : "",
    });

    return model;
  }

  downloadList() {
    var options = {
      fieldSeparator: ",",
      quoteStrings: '"',
      decimalseparator: ".",
      showLabels: true,
      headers: [
        "Transation No.",
        "Order Type",
        "Status",
        "Ordered By",
        "Ordered To",
        " PO No.",
        "PO Date",
        "Ordered Date"
      ]
    };

    this._requestService.action = "assign/main/whdrnumber"


    var param = this.keyWord;
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

  }
}
