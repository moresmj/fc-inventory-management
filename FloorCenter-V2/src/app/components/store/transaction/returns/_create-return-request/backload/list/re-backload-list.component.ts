import { Component,ViewChild,Input,Output,EventEmitter} from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

/*import { ReturnService } from '@services/return/return.service';
import { Return } from '@models/return/return.model';*/

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
	selector: 'app-re-backload-list',
	templateUrl: 're-backload-list.html'
})



export class ReturnBackloadListComponent{

	public now: Date = new Date();

	module : string = "return";

	 errorMessage: any;
 	statusMessage : any;

 	deliveryItems : any;

	updateForm : FormGroup;
	allRecords : any;//Return[] = [];
	returnList : any;// Return[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;
	returnDetails : any;


	constructor(private fb: FormBuilder)
	{

	}

	@ViewChild(PagerComponent)
		private pager : PagerComponent;


/*
	getreturnList(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
		this.returnList =  pagerModel["pageRecord"]; 
		this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
		this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
	
	}

	reloadRecord(event : string){

		  //this.statusMessage = event["statusMessage"];
	
    if (this.pager[event["pagerMethod"]]) {
      this.pager[event["pagerMethod"]]();    
    }
	}

	filterRecordWithParam(event : any) {
		this.pager["filterPageWithParams"](event);    
	}


	onBtnUpdateClick(data : any){

		this.statusMessage = null;
		console.log(data);
		this.returnDetails = data;

		if (data.order.orderType == 2 && data.order.deliveryType == 1) {
		      this.deliveryItems = data.soldItems;
		}
		else if (data.order.orderType == 2 && data.order.deliveryType == 2) {
		      this.deliveryItems = data.soldItems;
		}
		else {
		      this.deliveryItems = data.showroomDeliveries;
		}

		this.updateForm = this.fb.group({


			id : new FormControl(data.id),

	
			  siNumber : new FormControl(data.siNumber,Validators.required),
			  orNumber : new FormControl(data.orNumber,Validators.required),
			  drNumber : new FormControl(data.drNumber,Validators.required),
			  remarks : new FormControl(data.remarks),
			  ClientName : new FormControl(data.clientName,Validators.required),
			  address1: new FormControl(data.address1),
			  address2: new FormControl(data.address2),
			  address3: new FormControl(data.address3),
			  contactNumber : new FormControl(data.contactNumber),
			  releaseItems : this.fb.array([])


		});
	}



	downloadRecords(){
   var options = {
       fieldSeparator: ',',
       quoteStrings: '"',
       decimalseparator: '.',
       showLabels: true,
       headers: ['Transaction No.','Transaction','SI/PO No','OR Number','Release Date','Customer Name']   
      };
      let title = this.now;
      let record = this.allRecords.map(r => this.toModel(r));

      new Angular2Csv(record, title.toISOString(), options);
  }



  toModel(detail : any): return{
    let model = new return({

	 transactionNo	: detail.transactionNo,
	 transaction: detail.order.transactionTypeStr,						
	 SINumber	: detail.siNumber,	
	 ORNumber : detail.orNumber,
	 poDate : new Date(detail.order.poDate).toLocaleString(),									
	 ClientName :detail.clientName,								

    });
    return model;
  }
*/






}