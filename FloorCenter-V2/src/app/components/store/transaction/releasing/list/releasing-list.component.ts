import { Component,ViewChild,Input,Output,EventEmitter} from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, NgModel } from '@angular/forms';

import { ReleasingService } from '@services/releasing/releasing.service';
import { Releasing } from '@models/releasing/releasing.model';

import { PagerComponent } from '@common/pager/pager.component';
import 'rxjs/add/operator/map';

import { Angular2Csv } from 'angular2-csv/Angular2-csv';

@Component({
	selector: 'app-releasing-list',
	templateUrl: 'releasing-list.html'
})



export class ReleasingListComponent{

	public now: Date = new Date();

	module : string = "releasing";

	 errorMessage: any;
 	statusMessage : any;

 	deliveryItems : any;

	updateForm : FormGroup;
	allRecords : Releasing[] = [];
	releasingList : Releasing[] = [];
	totalRecordMessage : string;
	pageRecordMessage : string;
	releasingDetails : any;


	constructor(private fb: FormBuilder)
	{

	}

	@ViewChild(PagerComponent)
		private pager : PagerComponent;



	getReleasingList(pagerModel : any) {
		this.allRecords = pagerModel["allRecords"];
		this.releasingList =  pagerModel["pageRecord"]; 
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
		/*if(data.order == null)
		{
			  this.deliveryItems = data.soldItems;
		}
		else{
			if (data.order.orderType == 2 && data.order.deliveryType == 1) {
		      this.deliveryItems = data.soldItems;
			}
			else if (data.order.orderType == 2 && data.order.deliveryType == 2) {
			      this.deliveryItems = data.soldItems;
			}
			else {
			      //this.deliveryItems = data.showroomDeliveries;
				 this.deliveryItems = data.soldItems;
			}

		}*/

		
					/*if(data.salesType == 3){

						this.updateForm = this.fb.group({


								
								  id : new FormControl(data.id),
								  siNumber : new FormControl(data.siNumber,Validators.required),
								  orNumber : new FormControl("",Validators.required),
								  drNumber : new FormControl("",Validators.required),
								  remarks : new FormControl(data.remarks),
								  ClientName : new FormControl("",Validators.required),
								  address1: new FormControl(data.address1),
								  address2: new FormControl(data.address2),
								  address3: new FormControl(data.address3),
								  contactNumber : new FormControl(data.contactNumber),
								  releaseItems : this.fb.array([])


							});

					}*/

				

						this.updateForm = this.fb.group({


								  id : new FormControl(data.id),
								  siNumber : new FormControl(data.siNumber,Validators.required),
								  orNumber : new FormControl(data.orNumber,Validators.required),
								  drNumber : new FormControl(data.drNumber),
								  remarks : new FormControl(data.remarks),
								  ClientName : new FormControl(data.clientName,Validators.required),
								  address1: new FormControl(data.address1),
								  address2: new FormControl(data.address2),
								  address3: new FormControl(data.address3),
								  contactNumber : new FormControl(data.contactNumber),
								  salesAgent : new FormControl(data.salesAgent,Validators.required),
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
       headers: ['Transaction No.','SI No.','OR No.','Release Date','Customer Name']   
      };
      let title = this.now;
      let record = this.allRecords.map(r => this.toModel(r));

      new Angular2Csv(record, title.toISOString(), options);
  }



  toModel(detail : any): Releasing{
		  	if(detail.salesType == 3){
		    let model = new Releasing({
		    
		     transactionNo	: detail.transactionNo,				
			 SINumber	: detail.siNumber,	
			 ORNumber : detail.orNumber,
			 poDate : this.checkValue(detail.releaseDate),									
			 ClientName :detail.clientName,	


		    });
		       return model;
		  }

		    else{
		     let model = new Releasing({
		     transactionNo	: detail.transactionNo,						
			 SINumber	: detail.siNumber,	
			 ORNumber : detail.orNumber,
			 poDate : this.checkValue(detail.releaseDate),									
			 ClientName :detail.clientName
			});
		       return model;

		    }
 
  }

  checkValue(value :any)
  {

			if(value == null)
			{
				
			  		return null;

			}

			return new Date(value).toLocaleDateString().slice(0,10).replace(",","")
  	
  }







}