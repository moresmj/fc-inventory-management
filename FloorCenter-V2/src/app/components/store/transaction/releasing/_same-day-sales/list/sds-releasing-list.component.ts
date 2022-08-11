import { Component,ViewChild,Input,Output,EventEmitter} from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { SameDaySalesReleasingService } from '@services/releasing/same-day-sales-releasing.service';

import { Releasing } from '@models/releasing/releasing.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';
import { PagerNewComponent } from '@components/common/pagernew/pagernew.component';
import { BaseComponent } from '@components/common/base.component';
import { RequestService } from '@services/request.service';
import { CommonViewService } from '@services/common/common-view.service';

@Component({
	selector: 'app-sds-releasing-list',
	templateUrl: 'sds-releasing-list.html'
})



export class SameDaySalesReleasingListComponent extends BaseComponent{

	public now: Date = new Date();

	module : string = "same-day-sales-releasing";

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
				private _commonViewService : CommonViewService)
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
		
		this.pager["filterPageWithParams"](1,event);    
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
				siNumber : new FormControl(data.siNumber,[Validators.required, Validators.maxLength(50)]),
				orNumber : new FormControl(data.orNumber,[ Validators.maxLength(50)]),
				drNumber : new FormControl(data.drNumber,[Validators.maxLength(50)]),
				remarks : new FormControl(data.remarks),
				ClientName : new FormControl(data.clientName,Validators.required),
				address1: new FormControl(data.address1),
				address2: new FormControl(data.address2),
				address3: new FormControl(data.address3),
				contactNumber : new FormControl(data.contactNumber),
				salesAgent : new FormControl(data.salesAgent,Validators.required),
				deliveryType : new FormControl(data.deliveryType),
				transactionNo : new FormControl(data.transactionNo),
				releaseItems : this.fb.array([])


		});

				
		


	}



	downloadRecords(){
   var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction No.','SI No.','OR No.','Release Date','Customer Name']   
	  };
	  

	  this._requestService.action = "transactions/releasing/samedaysales";
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


    //   let title = this.now;
    //   let record = this.allRecords.map(r => this.toModel(r));

    //   new Angular2Csv(record, title.toISOString(), options);
  }



  toModel(detail : any): Releasing{
		 
		     let model = new Releasing({
		     transactionNo	: detail.transactionNo,						
			 SINumber	: detail.siNumber,	
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

			if(value == null)
			{
				
			  		return null;

			}

			return new Date(value).toLocaleDateString()
  	
  }







}