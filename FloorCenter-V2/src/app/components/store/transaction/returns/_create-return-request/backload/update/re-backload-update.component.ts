import { Component,ViewChild,Input,Output,EventEmitter} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators,FormArray } from '@angular/forms';

import { ReleasingService } from '@services/releasing/releasing.service';
import { Releasing } from '@models/releasing/releasing.model';

import { CommonViewService } from '@services/common/common-view.service';


@Component({
	selector: 'app-re-backload-update',
	templateUrl: 're-backload-update.html'
})



export class ReturnBackloadUpdateComponent{

	/*@Input() updateForm : FormGroup;
	@Input() releasingDetails : any;
	@Input() deliveryItems : any;
	@Output() updatePage: EventEmitter<any> = new EventEmitter<any>();
	successMessage : any;
	errorMessage : any;


	ngOnChanges(changes: SimpleChanges){
		if(changes["updateForm"])
		{
			this.errorMessage = null;
			this.successMessage = null;
			$("#updateForm :input").prop("disabled", false);
            $("#btnSave").show(); 
		}
	}


	constructor(
		private fb : FormBuilder,
		private _releasingService: ReleasingService,
		private _commonViewService : CommonViewService)
	{

	}

	onSubmit()
	{
		let formData = this.updateForm.value;
		console.log(formData);

		this._releasingService.updateRecord(formData.id,formData)
		.subscribe(SuccessCode=>{
			console.log("success");

			let emitValue = {"pagerMethod" : "loadPageRecord", "statusMessage" : "Request has been Released and Updated" };

			this.successMessage ="Request has been Released and Updated";
			this.updatePage.emit(emitValue);
			//this.statusMessage.emit("Request has been Released and Updated");
			//$("#details_modal").modal("hide");
			//$('body').removeClass('modal-open');
			//$('.modal-backdrop').remove();
			$("#updateForm :input").prop("disabled", true);
            $("#btnSave").hide(); 
            $("#btnCancel").prop("disabled", false);

		},
		error =>{
			       this.errorMessage = this._commonViewService.getErrors(error);
		});

	}



	 get releaseItems(): FormArray{
        return this.updateForm.get('releaseItems') as FormArray;
    }




*/




	
}