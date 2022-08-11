import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { ReleaseItems } from '@models/release-items/release-items.model';
import { RequestService } from '@services/request.service'

import { CommonViewService } from '@services/common/common-view.service';
import { AppConstants } from '@common/app-constants/app-constants';


declare var jquery:any;
declare var $ :any;
@Component({

	selector:'app-change-tonality-details',
	templateUrl: 'change-tonality-details.html'

})


export class ChangeTonalityDetailsComponent implements OnChanges {


	@Input()id:number;
	@Input()releaseItemDetails : any;
	@Input()updateForm: FormGroup;
	@Input()forReleasingItems : any;
	@Input()forReleasingItems2 : any;
	@Input()items: any;
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();
	successMessage : any;
	errorMessage : any;
	@Input()itemList : any;


	StatusMsg : string;
	inventoryDetail: any = [];
	// itemList : any =[];

    constructor(private fb: FormBuilder,
    	private _releaseItemsService : ReleaseItemsService,
		private _commonViewService : CommonViewService,
		private _requestService : RequestService){

			this._requestService.action = "transactions/releaseitems/changetonality";

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

		get ModifyItemTonalityDetails(): FormArray{
		return this.updateForm.get("ModifyItemTonalityDetails") as FormArray;
	}

	onChange(event : any)
	{
		this.errorMessage = null;
		this.successMessage = null;
	}


	addItems(data: any){
		if(data.id != null)
		{
		const control = <FormArray>this.updateForm.controls['ModifyItemTonalityDetails'];
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


		

		this._requestService.newRecord(formData)
		.subscribe(SuccessCode=>{
			console.log("success");

			let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been added" };

			this.updatePage.emit(emitValue);
			$("#btnSave").hide(); 


			this.successMessage = AppConstants.ChangeTonalitySucess;
			this.errorMessage = null;
		},
		error =>{
			this.errorMessage = this._commonViewService.getErrors(error);
		});
	}

	onClose(){

		this.releaseItemDetails = null;

	}



	   get deliveredItems(): FormArray{
        return this.updateForm.get('deliveredItems') as FormArray;
    }

    
}