import { Component,ViewChild,Input,Output,EventEmitter} from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { ReleasingService } from '@services/releasing/releasing.service';
import { Releasing } from '@models/releasing/releasing.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { RequestService } from '@services/request.service';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { BaseComponent } from '@components/common/base.component';
import { CommonViewService } from '@services/common/common-view.service';

@Component({
	selector: 'app-t-releasing-list',
	templateUrl: 't-releasing-list.html'
})



export class TransferReleasingListComponent extends BaseComponent{

	public now: Date = new Date();

	module : string = "transfer-releasing";

	 errorMessage: any;
 	statusMessage : any;

 	deliveryItems : any;

	updateForm : FormGroup;
	allRecords : Releasing[] = [];
	releasingList : Releasing[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;
	releasingDetails : any;


	constructor(private fb: FormBuilder,
				private _requestService : RequestService,
				private _commonViewService : CommonViewService,)
	{
		super();

	}

	@ViewChild(PagerNewComponent)
		private pager : PagerNewComponent;



	getReleasingList(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
		this.releasingList =  pagerModel["pageRecord"]; 
		this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
		this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
	
	}

	reloadRecord(event : string){
	
    if (this.pager[event["pagerMethod"]]) {
      this.pager[event["pagerMethod"]]();    
    }
	}

	filterRecordWithParam(event : any) {

		this.Keyword = event;
        this.Keyword["currentPage"] = 1;
		this.pager["filterPageWithParams"](1,this.Keyword);    
	}


	onBtnUpdateClick(data : any){

		this.statusMessage = null;
		console.log(data);
		this.releasingDetails = data;

		this.deliveryItems = data.soldItems;

		if(this.releasingDetails.salesType == 2 && this.releasingDetails.deliveryType == 3)
		{
			this.releasingDetails["csPickupFlg"] = true;
		}
		else
		{
			this.releasingDetails["csPickupFlg"] = false;
		}
		
				

		this.updateForm = this.fb.group({


				id : new FormControl(data.id),
				deliveryType : new FormControl(data.deliveryType),
				releaseItems : this.fb.array([])


		});

				
		


	}



	downloadRecords(){
	    var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['Transaction No.','SI No.','Client SI No.','PO No.','DR No.','TOR No.','Release Date','Customer Name']   
			};
	

	  this._requestService.action = "transactions/releasing/transfer";
	  this.Keyword["showAll"] = true;
	  var param = this.Keyword;
  
	  this._requestService.getListWithParams(param)
			  .subscribe(list =>{

					this.Keyword["showAll"] = false;
					let record = list["list"].map(r => this.toModel(r));
					let title = this.now;
					new Angular2Csv(record, title.toISOString(), options);
				  
			  },
			  error =>{
				this.errorMessage = this._commonViewService.getErrors(error);
			  });

  }



  toModel(detail : any): Releasing{

		let model = new Releasing({
			transactionNo	: detail.transactionNo,						
			SINumber	: detail.siNumber,	
			ClientSINumber : '', // The API itslef dont have a return parameter for this.
			PONumber : detail.poNumber,
			DRNumber : detail.interBranch ? "" : detail.drNumber,
			ORNumber : detail.orNumber,
			poDate : this.checkValue(detail.releaseDate),									
			ClientName :detail.clientName
		});

		Object.getOwnPropertyNames(model).forEach(key => {
		  model[key] = (model[key]) ? model[key] : '';
		});

		return model;	
  }

  checkValue(value :any)
  {
  	return (value) ? new Date(value).toLocaleDateString() : '';
  }







}