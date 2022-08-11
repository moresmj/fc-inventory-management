import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';
import { ApiBaseService } from '@services/api-base.service';
import { RequestService } from '@services/request.service';





@Component({

	selector: 'app-ai-details',
	templateUrl: 'ai-details.html'
})



export class AdjustInventoryDetailsComponent implements OnInit{

Adjustment : any;
adjustmentDetails : any;
UpdateForm : FormGroup;
@Output()updateList : EventEmitter<any> = new EventEmitter<any>();


successMessage : string;
errorMessage : any;




	constructor(
		private _apiBaseService : ApiBaseService,
		private _commonViewService : CommonViewService,
		private route: ActivatedRoute,
    private location: Location,
		private fb : FormBuilder,
		private _requestService : RequestService,
		)
	{

		this._apiBaseService.action = "imports/physicalcount/store/";
		this._requestService.action = "imports/physicalcount/store/";

	}

	ngOnInit()
	{
		this.getDeliveryReturn();
	}


	getDeliveryReturn(){

			const id = +this.route.snapshot.paramMap.get('id');
			    

			this._apiBaseService.getRecordWithId(id)
			    .subscribe(adj =>{
			    	this.Adjustment = adj;
			    	this.adjustmentDetails = adj.details;

			    	this.newForm(adj);
			    	
			    	console.log(adj);
			    

			    });

	    	}


	onSubmit(){
		let formValue = this.UpdateForm.value;
		console.log(formValue);

		 const id = +this.route.snapshot.paramMap.get('id');


      this._requestService.updateRecord(id,formValue)
        .subscribe(successCode =>{
  
          	this.successMessage = "Adjustment Succesfully Added.";
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

			 if(!$('.submit').is(':visible'))
			 {
			 	return;
			 }

			 $('.submit').hide();
		}
		else
		{
			if($('.submit').is(':visible'))
			{
			   return;
			}
			

			$('.submit').show();

		}
	}


	onChange(ch : any){


          	this.successMessage = null;
          	this.errorMessage = null;
          	this.hideSubmitBtn("show");
	}


	newForm(data : any){
		this.UpdateForm = this.fb.group({
			 Id : new FormControl(data.id),
			 Details : this.fb.array([])


		});


		        	for(let i = 0; i < data["details"].length; i++)
				   {

					   	const control = <FormArray>this.UpdateForm.controls['Details'];

					   	let item = data["details"][i];


					   	let newItem = this.fb.group({
					   	id : new FormControl(item["id"]),						
						  stimportid	: new FormControl(data.id),													
					   	itemId : new FormControl(item["itemId"]),
					   	allowUpdate : new FormControl(1)



					   	})

					   	control.push(newItem);


				   }




	}


	get Details(): FormArray{
        return this.UpdateForm.get('Details') as FormArray;
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
