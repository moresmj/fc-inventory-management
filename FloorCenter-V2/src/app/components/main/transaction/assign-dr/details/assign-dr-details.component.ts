import { Component,Input, Output, EventEmitter, SimpleChanges, OnChanges  } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { ApiBaseService } from '@services/api-base.service';
import { CommonViewService } from '@services/common/common-view.service';
import { RequestService } from '@services/request.service';

@Component({
	selector: "app-assign-dr-details",
	templateUrl : "./assign-dr-details.html"
})

export class AssignDrDetailsComponent {
	
	@Input() showSaveBtn : boolean;
	@Input() isClient : boolean;
	@Input() delivery : any;
	@Input() updateForm : FormGroup
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();

	successMessage : string;	
	errorMessage : any;

  constructor(private _deliveriesService : ApiBaseService, 
              private _commonViewService : CommonViewService,
              private _requestService : RequestService)
	{
		this._deliveriesService.action = "assignmain/whdrnumber/";

	}

	onSubmit() {

    this._requestService.action = "assign/main/whdrnumber";
    let formData = this.updateForm.value;
    
    this._requestService.updateRecord(this.delivery.id,formData)
        .subscribe(successCode => {
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Updated";
            this.errorMessage = null;  

            $("#btnSave").hide();
            $("#btnCancel").hide(); 
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });

  }
  
  onCancel(){
    this._requestService.action = "assign/main/whdrnumber";
    let formData = this.updateForm.value;
    formData.RequestStatus = 3;

    this._requestService.updateRecord(this.delivery.id,formData)
        .subscribe(successCode => {
            this.updatePage.emit("loadPageRecord");

            this.successMessage = "Record Succesfully Cancelled";
            this.errorMessage = null;  

            $("#btnCancel").hide();
            $("#btnSave").hide();
        },
        error =>{
             this.errorMessage = this._commonViewService.getErrors(error);
             this.successMessage = null;
        });
  }


	onChange() {
      	this.successMessage = null;
      	this.errorMessage = null;
	}



	ngOnChanges(changes : SimpleChanges)
	{
    $("#btnSave").show();
    $("#btnCancel").show();

		this.successMessage = null;
		this.errorMessage = null;
	}

	print(): void {
    let printContents,printContents1,printContents2, printContents3,  popupWin,title,headContent;
    headContent = document.getElementsByTagName('head')[0].innerHTML;
    printContents = document.getElementById('printable').innerHTML;

    //clone and format html for printing
    var printobject = $("#printable");
    printobject = printobject.clone();
    printContents  =  $(printContents).find('[name=divRemove]').contents().unwrap();

    printContents1 = printContents.prevObject.prevObject[0].innerHTML;
    printContents3 = printContents.prevObject.prevObject[2].innerHTML;
    printContents3 = printContents3.replace("table-responsive","");
    title = document.getElementById("po").innerHTML;
    popupWin = window.open('', '_blank', 'top=0,left=0,height=auto,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>${title}</title>
          ${headContent}
          
        </head>
    <body onload="window.print();window.close() ">${printContents1}
    ${printContents3}</body>
      </html>`
    );
    popupWin.document.close();
}


}