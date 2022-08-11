import { Component,OnInit,Output,Input, OnChanges, SimpleChanges,EventEmitter} from '@angular/core';
import { FormGroup,FormControl,FormBuilder,Validators,FormArray } from '@angular/forms';

import { CommonViewService } from '@services/common/common-view.service';
import { ApiBaseService } from '@services/api-base.service';
import { RequestService } from '@services/request.service';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({

	selector: 'app-re-dr-add',
	templateUrl: 're-dr-add.html'
})



export class ReturnDeliveryRequestAddComponent implements OnChanges{

@Input() returnedItems : any;
@Input() returnAddForm : FormGroup;
@Output()updateList : EventEmitter<any> = new EventEmitter<any>();


successMessage : string;
errorMessage : any;




	constructor(
		private _apiBaseService : ApiBaseService,
		private _commonViewService : CommonViewService,
		private route: ActivatedRoute,
		private _requestService :RequestService,
    private location: Location)
	{

		this._apiBaseService.action = "returns/rtv/delivery/";


	}


	get WarehouseDeliveries(): FormArray{
		return this.returnAddForm.get("WarehouseDeliveries") as FormArray;
	}


	onSubmit(){
		this._requestService.action = "returns/rtv/delivery";
		let formValue = this.returnAddForm.value;
		console.log(formValue);

		 const id = +this.route.snapshot.paramMap.get('id');


        this._requestService.updateRecord(id,formValue)
        .subscribe(successCode =>{
  
          	this.successMessage = "Showroom Delivery Succesfully Added.";
          	this.errorMessage = null;
          	this.updateList.emit(successCode);
          	this.hideSubmitBtn("hide");

        },
        error =>{
            this.errorMessage = this._commonViewService.getErrors(error);
            this.successMessage = null;
        });
	
	}

	hideSubmitBtn(condition : any){
		if(condition == "hide")
		{

			 if(!$('#submit').is(':visible'))
			 {
			 	return;
			 }

			 $('#submit').hide();
		}
		else
		{
			if($('#submit').is(':visible'))
			{
			   return;
			}
			

			$('#submit').show();

		}
	}


	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
          	this.hideSubmitBtn("show");
	}


	

ConvertToInt(quantity){
  return parseInt(quantity);
}

	ngOnChanges(changes : SimpleChanges)
	{

		this.successMessage = null;
		this.errorMessage = null;
		this.hideSubmitBtn("show");
	}

	/*



	


	get ShowroomDeliveries(): FormArray{
		return this.showRoomForm.get("ShowroomDeliveries") as FormArray;
	}

	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
          	this.hideSubmitBtn("show");
	}


	



	ngOnChanges(changes : SimpleChanges)
	{

		this.successMessage = null;
		this.errorMessage = null;
	}

		

	

*/

	
}