import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { ReleaseItems } from '@models/release-items/release-items.model';
import { RequestService } from '@services/request.service'

import { CommonViewService } from '@services/common/common-view.service';


declare var jquery:any;
declare var $ :any;
@Component({

	selector:'app-release-item-details',
	templateUrl: 'release-item-details.html'

})


export class ReleaseItemDetailsComponent implements OnChanges {


	@Input()id:number;
	@Input()releaseItemDetails : any;
	@Input()updateForm: FormGroup;
	@Input()forReleasingItems : any;
	@Input()forReleasingItems2 : any;
	@Input()items: any;
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();
	successMessage : any;
	errorMessage : any;

	//@Output() statusMessage : EventEmitter<String> = new EventEmitter<String>();

	StatusMsg : string;
	inventoryDetail: any = [];
	itemList : any =[];

    constructor(private fb: FormBuilder,
    	private _releaseItemsService : ReleaseItemsService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService){

			this._requestService.action = "transactions/releaseitems";

    }

	ngOnChanges(changes : SimpleChanges)
	{
		
		if(changes["updateForm"])
		{
			this.errorMessage = null;
			this.successMessage = null;
			$("#updateForm :input").prop("disabled", false);
            $("#btnSave").show(); 
		}
          
	}

		get forReleasing(): FormArray{
		return this.updateForm.get("forReleasing") as FormArray;
	}


	addItems(data: any){
		if(data.id != null)
		{
		const control = <FormArray>this.updateForm.controls['forReleasing'];
		let newItemRow = this.fb.group({

			"id":[data["id"],Validators.required],
			"whInventoryId":[data["whInventoryId"],Validators.required],
			"quantity" : [data["quantity"],Validators.required],
			"remarks":[data["remarks"],Validators.required],
			"dateCreated": [data["dateCreated"],Validators.required],
			"dateUpdated" :[data["dateCreated"]]




		})

	
		this.inventoryDetail.push(data);
		let index = this.inventoryDetail.length - 1;
		this.inventoryDetail[index]["code"]  = data["item"].code;
		this.inventoryDetail[index]["itemName"]  = data["item"].name;
		this.inventoryDetail[index]["size"]  = data["item"].size.name;
		this.inventoryDetail[index]["tonality"]  = data["item"].tonality;
		
	

		control.push(newItemRow);
		}


	}

	onSubmit(){
		let formData = this.updateForm.value;
		console.log(formData);


		/*if(formData.deliveryType == 1) || (formData.orderType == 2 && formData.deliveryType == 2)
		{
			let action = "/clientorder"
			this._releaseItemsService.updateRecord(formData.id,formData,action)
		.subscribe(SuccessCode=>{
			console.log("success");

			let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

			this.updatePage.emit(emitValue);
			//this.statusMessage.emit("Request has been Released and Updated");
			//$("#details_modal").modal("hide");
			//$('body').removeClass('modal-open');
			//$('.modal-backdrop').remove();
			//$("#updateForm :input").prop("disabled", true);
			//$("#btnCancel").prop("disabled", false);


            $("#btnSave").hide(); 
   

			this.successMessage = "Request has been Approved and Updated";
		},
		error =>{
			  this.errorMessage = this._commonViewService.getErrors(error);
		});

		}
		else{*/
		this._requestService.updateRecord(formData.id,formData)
		.subscribe(SuccessCode=>{
			console.log("success");

			let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

			this.updatePage.emit(emitValue);
			$("#btnSave").hide(); 


			this.successMessage = "Request has been Approved and Updated";
		},
		error =>{
			this.errorMessage = this._commonViewService.getErrors(error);
		});
	
		// this._releaseItemsService.updateRecord(formData.id,formData,"")
		// .subscribe(SuccessCode=>{
		// 	console.log("success");

		// 	let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

		// 	this.updatePage.emit(emitValue);
		// 	//this.statusMessage.emit("Request has been Released and Updated");
		// 	//$("#details_modal").modal("hide");
		// 	//$('body').removeClass('modal-open');
		// 	//$('.modal-backdrop').remove();
		// 	//$("#updateForm :input").prop("disabled", true);
        //     $("#btnSave").hide(); 
        //     //$("#btnCancel").prop("disabled", false);

		// 	this.successMessage = "Request has been Approved and Updated";
		// },
		// error =>{
		// 	  this.errorMessage = this._commonViewService.getErrors(error);
		// });

		//}


	
	}

	onClose(){

		this.releaseItemDetails = null;

	}



	   get deliveredItems(): FormArray{
        return this.updateForm.get('deliveredItems') as FormArray;
    }

    
}